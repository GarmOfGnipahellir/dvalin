using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Machine))]
public class Crusher : MonoBehaviour
{
    protected const string k_QueuedName = "queued";

    public Switch inputSwitch;
    public int queueCapacity = 10;

    protected ZNetView m_Nview;
    protected Machine m_Machine;

    public ZDO zdo { get { return m_Nview.GetZDO(); } }

    void Awake()
    {
        m_Nview = GetComponent<ZNetView>();
        m_Machine = GetComponent<Machine>();

        inputSwitch.m_onUse += OnInputUsed;

        m_Nview.Register("InputUsed", RPC_InputUsed);
    }

    void FixedUpdate()
    {
        if (!m_Nview.IsValid()) return;

        UpdateHoverTexts();
    }

    protected void UpdateHoverTexts()
    {
        inputSwitch.m_hoverText = string.Format(
            "$dvalin_piece_crusher ({0}/{1})",
            zdo.GetInt(k_QueuedName),
            queueCapacity
        );
    }

    protected bool OnInputUsed(Switch sw, Humanoid user, ItemDrop.ItemData item)
    {
        Debug.LogFormat("{0} used by {1} with {2}", sw, user, item);

        m_Nview.InvokeRPC("InputUsed");

        return true;
    }

    protected void RPC_InputUsed(long sender)
    {
        if (!m_Nview.IsOwner()) return;

        Debug.LogFormat("Crusher input used sent by {0}", sender);

        zdo.Set(k_QueuedName, zdo.GetInt(k_QueuedName) + 1);
    }
}
