using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour
{
    public CableMount cableMount;
    public ItemInput input;
    public ItemOutput output;

    private ZNetView m_Nview;

    public ZNetView nview
    {
        get
        {
            if (!m_Nview)
            {
                m_Nview = GetComponentInParent<ZNetView>();
                Debug.Assert(m_Nview);
            }
            return m_Nview;
        }
    }
    public ZDO zdo { get { return nview.GetZDO(); } }
}
