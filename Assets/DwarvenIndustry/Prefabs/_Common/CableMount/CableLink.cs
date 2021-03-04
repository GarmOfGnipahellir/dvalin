using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CableLink : MonoBehaviour
{
    const float k_PositionGap = 0.3f;
    const float k_CableWidth = 0.15f;
    const float k_CableSlack = 0.5f;

    public CableMount mount1;
    public CableMount mount2;

    private LineRenderer m_Renderer;

    void Awake()
    {
        m_Renderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        RebuildLine();
    }

    public void RebuildLine()
    {
        Vector3 mp1 = mount1.mountPoint.position;
        Vector3 mp2 = mount2.mountPoint.position;

        float distance = (mp2 - mp1).magnitude;
        int numPositions = (int)(distance / k_PositionGap);
        float normalizedLength = distance / CableMount.k_ConnectRange;

        Vector3[] positions = new Vector3[numPositions];
        for (int i = 0; i < numPositions; i++)
        {
            float a = Mathf.InverseLerp(0, numPositions - 1, i);
            float t = a * 2 - 1;
            positions[i] =
                Vector3.Lerp(mp1, mp2, a) +
                Vector3.down * Mathf.Abs(t * t - 1) * 
                k_CableSlack * normalizedLength;
        }

        m_Renderer.positionCount = numPositions;
        m_Renderer.SetPositions(positions);
    }
}
