using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CableMount : MonoBehaviour
{
    public const float k_ConnectRange = 10;

    public CableLink linkPrefab;
    public Transform mountPoint;

    protected List<CableLink> m_Links = new List<CableLink>();

    void Start()
    {
        ReconnectInRange();
    }

    void OnDestroy()
    {
        DisconnectAll();
    }

    public void ReconnectInRange()
    {
        DisconnectAll();

        foreach (var mount in FindInRadius(k_ConnectRange))
        {
            Connect(mount);
        }
    }

    public void Connect(CableMount other)
    {
        if (IsConnected(other)) return;

        var link = Object.Instantiate<CableLink>(linkPrefab);
        link.mount1 = this;
        link.mount2 = other;

        m_Links.Add(link);
        other.m_Links.Add(link);
    }

    public void DisconnectAll()
    {
        foreach (var link in m_Links.ToArray())
        {
            Disconnect(link);
        }
    }

    public bool IsConnected(CableMount other)
    {
        return m_Links.Exists(x => x.mount1 == other || x.mount2 == other);
    }

    public CableMount[] FindInRadius(float radius)
    {
        return FindInRadius(transform.position, radius)
            .Where(x => x != this)
            .ToArray();
    }

    public static CableMount[] FindInRadius(Vector3 origin, float radius)
    {
        var layerMask = LayerMask.GetMask("piece");
        var colliders = Physics.OverlapSphere(origin, radius, layerMask);
        return colliders
            .Select(x => x.GetComponentInParent<CableMount>())
            .Where(x => x != null)
            .ToArray();
    }

    public static void Disconnect(CableLink link)
    {
        link.mount1.m_Links.Remove(link);
        link.mount2.m_Links.Remove(link);

        Object.Destroy(link.gameObject);
    }
}