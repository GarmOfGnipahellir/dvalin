using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(Machine), typeof(Piece))]
public class Crusher : MonoBehaviour
{
    public Switch inputSwitch;
    public int queueCapacity = 10;
    public float processDuration = 3f; // ANCHOR maybe use num cycles instead?

    protected ZNetView m_Nview;
    protected Machine m_Machine;
    protected Piece m_Piece;

    public ZDO zdo { get { return m_Nview.GetZDO(); } }
    public int queuedCount
    {
        get
        {
            return zdo.GetInt(nameof(queuedCount));
        }
        set
        {
            zdo.Set(nameof(queuedCount), value);
        }
    }
    public float processTime
    {
        get
        {
            return zdo.GetFloat(nameof(processTime));
        }
        set
        {
            zdo.Set(nameof(processTime), value);
        }
    }

    void Awake()
    {
        m_Nview = GetComponent<ZNetView>();
        m_Machine = GetComponent<Machine>();
        m_Piece = GetComponent<Piece>();

        inputSwitch.m_onUse += OnInputUsed;

        m_Nview.Register("InputUsed", RPC_InputUsed);
        InvokeRepeating("UpdateCrusher", 1f, 1f); // ANCHOR why not coroutine?
    }

    void FixedUpdate()
    {
        if (!m_Nview.IsValid()) return;

        UpdateHoverTexts();
    }

    protected void UpdateHoverTexts()
    {
        inputSwitch.m_hoverText = string.Format(
            "{0} ({1}/{2})\n{3}",
            m_Piece.m_name,
            queuedCount,
            queueCapacity,
            processTime
        );
    }

    protected bool OnInputUsed(Switch sw, Humanoid user, ItemDrop.ItemData item)
    {
        if (queuedCount >= queueCapacity)
        {
            user.Message(MessageHud.MessageType.Center, "$msg_itsfull");
            return false;
        }

        user.Message(MessageHud.MessageType.Center, "$msg_added");
        m_Nview.InvokeRPC("InputUsed");
        return true;
    }

    protected void RPC_InputUsed(long sender)
    {
        if (!m_Nview.IsOwner()) return;

        queuedCount++;
    }

    protected void UpdateCrusher()
    {
        // ANCHOR could just not repeat this instead of checking every cycle?
        if (!m_Nview.IsValid() || !m_Nview.IsOwner()) return;

        // need to do this to always update last cycle time
        var deltaTime = GetDeltaTime(); // time since last cycle

        if (queuedCount <= 0) return;

        processTime += (float)deltaTime;

        if (processTime >= processDuration)
        {
            queuedCount--;
            processTime = 0f;
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
