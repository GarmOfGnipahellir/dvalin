using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(Machine))]
public class ThermalGenerator : MonoBehaviour
{
    public ItemInput input;
    public float secPerFuel = 10f;

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

    void Awake()
    {
        InvokeRepeating("BurnFuel", 1f, 1f);
    }

    protected void BurnFuel()
    {
        if (!nview.IsValid() || !nview.IsOwner()) return;

        var deltaTime = GetDeltaTime();

        if (input.counter <= 0) return;

        if ((burnTimer += (float)deltaTime) >= secPerFuel)
        {
            input.ConsumeFirstItem();
            burnTimer = 0;
        }
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
