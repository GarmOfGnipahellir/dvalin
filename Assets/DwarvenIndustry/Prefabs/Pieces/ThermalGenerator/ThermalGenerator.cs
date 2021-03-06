using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(Machine))]
public class ThermalGenerator : MonoBehaviour
{
    public ItemInput input;
    public float secPerFuel;

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
    public float burnTimer
    {
        get
        {
            return zdo.GetFloat(nameof(burnTimer));
        }
        set
        {
            zdo.Set(nameof(burnTimer), value);
        }
    }

    void Start()
    {
        StartCoroutine(ProcessFuel());
    }

    IEnumerator ProcessFuel()
    {
        yield return new WaitForSeconds(1f);

        var deltaTime = GetDeltaTime();

        StartCoroutine(ProcessFuel());
    }

    // from valheim smelter
    private double GetDeltaTime()
    {
        DateTime time = ZNet.instance.GetTime();
        DateTime dateTime = new DateTime(zdo.GetLong("StartTime", time.Ticks));
        double totalSeconds = (time - dateTime).TotalSeconds;
        zdo.Set("StartTime", time.Ticks);
        return totalSeconds;
    }
}
