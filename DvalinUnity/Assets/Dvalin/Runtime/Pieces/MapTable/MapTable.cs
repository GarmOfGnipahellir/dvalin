using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MapTable : MonoBehaviour
{
    public MeshRenderer mapMesh;
    public Material mapMaterial;

    public Texture2D forestMaskTexture;
    public Texture2D mapTexture;
    public Texture2D heightTexture;
    public Texture2D fogTexture;

    public int currentTexture = 0;
    string[] m_names = new[] { "Forest Mask Texture", "Map Texture", "Height Texture", "Fog Texture" };
    Texture2D[] m_textures;

    void Start()
    {
        SetupMap();

        // var sw = GetComponent<Switch>();
        // sw.m_onUse += new Switch.Callback(SwitchTexture);
        // sw.m_hoverText = m_textures[currentTexture].name;
    }

    Texture2D GetTextureFromMinimap(string fieldName)
    {
        return typeof(Minimap).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Minimap.instance) as Texture2D;
    }

    void SetupMap()
    {
        forestMaskTexture = GetTextureFromMinimap("m_forestMaskTexture");
        mapTexture = GetTextureFromMinimap("m_mapTexture");
        heightTexture = GetTextureFromMinimap("m_heightTexture");
        fogTexture = GetTextureFromMinimap("m_fogTexture");

        m_textures = new[] {
            forestMaskTexture,
            mapTexture,
            heightTexture,
            fogTexture,
        };

        // debug save textures
        // int i = 0;
        // foreach (var texture in m_textures)
        // {
        //     try
        //     {
        //         var bytes = texture.EncodeToEXR();
        //         var file = Application.dataPath + "/../" + m_names[i] + ".exr";
        //         File.WriteAllBytes(file, bytes);
        //         Debug.Log("Saved texture " + file);
        //     }
        //     catch (System.Exception e)
        //     {
        //         Debug.LogWarning(e);
        //     }
        //     i++;
        // }

        var instMapMaterial = new Material(mapMaterial);
        instMapMaterial.SetTexture("_MainTex", mapTexture);
        instMapMaterial.SetTexture("_HeightTex", heightTexture);
        instMapMaterial.SetTexture("_FogTex", fogTexture);
        instMapMaterial.SetTexture("_MaskTex", forestMaskTexture);
        instMapMaterial.SetFloat("_WaterLevel", mapMaterial.GetFloat("_WaterLevel") * 10);

        var prevMat = mapMesh.materials[1];
        var instPieceMaterial = new Material(Shader.Find("Custom/Piece"));
        instPieceMaterial.SetTexture("_MainTex", prevMat.GetTexture("_MainTex"));
        instPieceMaterial.SetTexture("_BumpMap", prevMat.GetTexture("_BumpMap"));
        instPieceMaterial.SetFloat("_Glossiness", prevMat.GetFloat("_Glossiness"));

        mapMesh.materials = new[] { instMapMaterial, instPieceMaterial };
    }

    // bool SwitchTexture(Switch sw, Humanoid user, ItemDrop.ItemData item)
    // {
    //     currentTexture = (currentTexture + 1) % m_textures.Length;
    //     mapMesh.material.SetTexture("_MainTex", m_textures[currentTexture]);
    //     sw.m_hoverText = m_textures[currentTexture].name;
    //     return true;
    // }
}
