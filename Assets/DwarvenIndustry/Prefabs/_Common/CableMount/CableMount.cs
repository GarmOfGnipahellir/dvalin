using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CableMount : MonoBehaviour
{
    public CableLink linkPrefab;
    public Transform mountPoint;
    public float connectRange = 10;

    protected List<CableLink> m_Links = new List<CableLink>();

    void Start()
    {
        var nearbyMounts = FindInRadius(connectRange);
        foreach (var nearbyMount in nearbyMounts)
        {
            Connect(nearbyMount);
        }
    }

    public void Connect(CableMount other)
    {
        if (!IsConnected(other))
        {
            var link = Object.Instantiate<CableLink>(linkPrefab);
            link.mount1 = this;
            link.mount2 = other;

            m_Links.Add(link);
            other.m_Links.Add(link);
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
}