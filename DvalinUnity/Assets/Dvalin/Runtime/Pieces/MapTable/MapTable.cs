using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MapTable : MonoBehaviour
{
    static readonly Color32 WHITE = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
    static readonly Color32 BLACK = new Color32(byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue);

    public MeshRenderer m_mapMesh;
    public Material m_mapMaterial;

    public Texture2D m_forestMaskTexture;
    public Texture2D m_mapTexture;
    public Texture2D m_heightTexture;
    public Texture2D m_fogTexture;
    public bool[] m_explored;

    public ZNetView m_nview;

    public int TextureSize { get { return Minimap.instance.m_textureSize; } }

    void Start()
    {
        m_nview = GetComponent<ZNetView>();
        if (!m_nview)
        {
            Debug.LogError("No ZNetView found on map table.");
            return;
        }

        m_explored = new bool[TextureSize * TextureSize];
        for (int i = 0; i < m_explored.Length; i++)
        {
            m_explored[i] = false;
        }
        if (m_nview.GetZDO() != null)
        {
            var exploration = m_nview.GetZDO().GetByteArray("exploration");
            if (exploration != null && exploration.Length == m_explored.Length)
            {
                for (int i = 0; i < m_explored.Length; i++)
                {
                    m_explored[i] = exploration[i] > byte.MinValue;
                }
            }
        }

        SetupMap();

        var sw = GetComponent<Switch>();
        sw.m_onUse += new Switch.Callback(UpdateMap);
        sw.m_hoverText = "[<color=yellow><b>$KEY_Use</b></color>] $dvalin_piece_maptable_use";
    }

    Texture2D GetTextureFromMinimap(string fieldName)
    {
        return typeof(Minimap).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Minimap.instance) as Texture2D;
    }

    void SetupMap()
    {
        m_forestMaskTexture = GetTextureFromMinimap("m_forestMaskTexture");
        m_mapTexture = GetTextureFromMinimap("m_mapTexture");
        m_heightTexture = GetTextureFromMinimap("m_heightTexture");
        m_fogTexture = new Texture2D(TextureSize, TextureSize, TextureFormat.RGBA32, false);
        m_fogTexture.wrapMode = TextureWrapMode.Clamp;
        UpdateFogTexture();

        var prevMat = m_mapMesh.materials[0];
        var northMaterial = new Material(Shader.Find("Custom/Piece"));
        northMaterial.SetTexture("_MainTex", null);
        northMaterial.SetTexture("_BumpMap", null);
        northMaterial.SetFloat("_Glossiness", prevMat.GetFloat("_Glossiness"));
        northMaterial.SetColor("_Color", prevMat.GetColor("_Color"));

        var instMapMaterial = new Material(m_mapMaterial);
        instMapMaterial.SetTexture("_MainTex", m_mapTexture);
        instMapMaterial.SetTexture("_HeightTex", m_heightTexture);
        instMapMaterial.SetTexture("_FogTex", m_fogTexture);
        instMapMaterial.SetTexture("_MaskTex", m_forestMaskTexture);
        instMapMaterial.SetFloat("_WaterLevel", ZoneSystem.instance.m_waterLevel);

        prevMat = m_mapMesh.materials[2];
        var woodMaterial = new Material(Shader.Find("Custom/Piece"));
        woodMaterial.SetTexture("_MainTex", prevMat.GetTexture("_MainTex"));
        woodMaterial.SetTexture("_BumpMap", prevMat.GetTexture("_BumpMap"));
        woodMaterial.SetFloat("_Glossiness", prevMat.GetFloat("_Glossiness"));

        m_mapMesh.materials = new[] { northMaterial, instMapMaterial, woodMaterial };
    }

    bool UpdateMap(Switch sw, Humanoid user, ItemDrop.ItemData item)
    {
        if (item != null)
        {
            Debug.Log("Items are not usuable on map table.");
            return false;
        }

        var player = user as Player;
        if (!player)
        {
            Debug.Log("Map table used by non-player humanoid.");
            return false;
        }

        if (!TryCombineExploration())
        {
            Debug.LogWarning("Couldn't combine eploration.");
            return false;
        }

        UpdateFogTexture();
        SaveExploration();

        Debug.Log("Map updated.");

        return true;
    }

    bool TryCombineExploration()
    {
        var minimapExplored = typeof(Minimap).GetField("m_explored", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Minimap.instance) as bool[];

        if (minimapExplored.Length != m_explored.Length)
        {
            Debug.LogError("Eploration array sizes doesn't match.");
        }

        for (int i = 0; i < m_explored.Length; i++)
        {
            var newExplored = CombineExploreBooleans(m_explored[i], minimapExplored[i]);
            m_explored[i] = newExplored;

            if (newExplored)
            {
                MethodInfo[] minimapFuncs = typeof(Minimap).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (MethodInfo minimapFunc in minimapFuncs)
                {
                    if (minimapFunc.Name != "Explore")
                    {
                        continue;
                    }

                    var parameters = minimapFunc.GetParameters();
                    if (parameters.Length < 2 || parameters[0].ParameterType != typeof(int) || parameters[1].ParameterType != typeof(int))
                    {
                        continue;
                    }
                    
                    minimapFunc.Invoke(Minimap.instance, new object[] { i % TextureSize, i / TextureSize });
                }
            }
        }
        GetTextureFromMinimap("m_fogTexture").Apply();

        return true;
    }

    bool CombineExploreBooleans(bool b1, bool b2)
    {
        int i1 = b1 ? 0 : 1;
        int i2 = b2 ? 0 : 1;
        return i1 * i2 == 0;
    }

    void UpdateFogTexture()
    {
        Color32[] colors = new Color32[TextureSize * TextureSize];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = m_explored[i] ? BLACK : WHITE;
        }
        m_fogTexture.SetPixels32(colors);
        m_fogTexture.Apply();
    }

    void SaveExploration()
    {
        byte[] exploration = new byte[TextureSize * TextureSize];
        for (int i = 0; i < exploration.Length; i++)
        {
            exploration[i] = m_explored[i] ? byte.MaxValue : byte.MinValue;
        }
        m_nview.GetZDO().Set("exploration", exploration);
    }
}
