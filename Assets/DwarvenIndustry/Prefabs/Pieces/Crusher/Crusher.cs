using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

[RequireComponent(typeof(Machine), typeof(Piece))]
public class Crusher : MonoBehaviour
{
    public Transform output;
    public float secPerProduct = 10f;
    public int dustPerOre = 2;
    public Smelter.ItemConversion[] conversions;

    private Machine m_Machine;
    private ZNetView m_Nview;

    public Machine machine
    {
        get
        {
            if (!m_Machine)
            {
                m_Machine = GetComponentInParent<Machine>();
                Debug.Assert(m_Machine);
            }
            return m_Machine;
        }
    }
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
    public float processTimer
    {
        get
        {
            return zdo.GetFloat(nameof(processTimer));
        }
        set
        {
            zdo.Set(nameof(processTimer), value);
        }
    }

    void Awake()
    {
        machine.input.validItems = conversions
            .Select(x => (Dvalin.ItemDropWrapper)x.m_from)
            .ToArray();

        InvokeRepeating("ProcessProduct", 1f, 1f);
    }

    protected void ProcessProduct()
    {
        if (!nview.IsValid() || !nview.IsOwner()) return;

        var deltaTime = GetDeltaTime();

        if (machine.input.counter <= 0) return;

        if ((processTimer += (float)deltaTime) >= secPerProduct)
        {
            Output(machine.input.firstItem);
            machine.input.ConsumeFirstItem();
            processTimer = 0;
        }
    }

    protected void Output(string itemName)
    {
        var conversion = conversions.Single(x => x.m_from.gameObject.name == itemName);
        if (conversion == null) return;

        machine.output.AddItem(conversion.m_to.gameObject.name, dustPerOre);
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
