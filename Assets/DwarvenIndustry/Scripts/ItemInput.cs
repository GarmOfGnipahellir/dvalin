using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ItemInput : MonoBehaviour
{
    public int capacity = 10;
    public Dvalin.ItemDropWrapper[] validItems;

    private Switch m_UseSwitch;
    private Piece m_Piece;
    private ZNetView m_Nview;

    public Switch useSwitch
    {
        get
        {
            if (!m_UseSwitch)
            {
                m_UseSwitch = GetComponent<Switch>();
                Debug.Assert(m_UseSwitch);
            }
            return m_UseSwitch;
        }
    }
    public Piece piece
    {
        get
        {
            if (!m_Piece)
            {
                m_Piece = GetComponentInParent<Piece>();
                Debug.Assert(m_Piece);
            }
            return m_Piece;
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
    public int counter
    {
        get
        {
            return zdo.GetInt(nameof(counter));
        }
        set
        {
            zdo.Set(nameof(counter), value);
        }
    }
    public string firstItem { get { return zdo.GetString("item0"); } }

    void Awake()
    {
        useSwitch.m_onUse += OnInputUsed;

        nview.Register<string>("InputUsed", RPC_InputUsed);
    }

    void FixedUpdate()
    {
        if (!nview.IsValid()) return;

        UpdateHoverTexts();
    }

    public string GetItem(int index) { return zdo.GetString(string.Format("item{0}", index)); }
    public void SetItem(int index, string value) { zdo.Set(string.Format("item{0}", index), value); }

    public ItemDrop.ItemData FindValidItem(Inventory inventory)
    {
        foreach (var validItem in validItems)
        {
            var item = inventory.GetItem(validItem.runtimeItemDrop.m_itemData.m_shared.m_name);
            if (item != null)
            {
                return item;
            }
        }
        return null;
    }

    public void ConsumeFirstItem()
    {
        if (counter <= 0) return;

        for (int i = 0; i < counter; i++)
        {
            SetItem(i, GetItem(i + 1));
        }
        counter--;
    }

    protected void UpdateHoverTexts()
    {
        useSwitch.m_hoverText = string.Format(
            // NOTE: $piece_smelter_additem translation might change!
            "{0} ({1}/{2})\n[<color=yellow><b>$KEY_Use</b></color>] $piece_smelter_additem",
            piece.m_name,
            counter,
            capacity
        );

        useSwitch.m_hoverText += "\n";

        for (int i = 0; i < counter; i++)
        {
            useSwitch.m_hoverText += "\n" + GetItem(i);
        }
    }

    protected bool OnInputUsed(Switch sw, Humanoid user, ItemDrop.ItemData item)
    {
        if (item == null)
        {
            item = FindValidItem(user.GetInventory());
            if (item == null)
            {
                user.Message(MessageHud.MessageType.Center, "$msg_noprocessableitems");
                return false;
            }
        }
        else if (!Array.Exists(validItems, x => x.name == item.m_dropPrefab.name))
        {
            user.Message(MessageHud.MessageType.Center, "$msg_wrongitem");
            return false;
        }

        if (counter >= capacity)
        {
            user.Message(MessageHud.MessageType.Center, "$msg_itsfull");
            return false;
        }

        user.Message(MessageHud.MessageType.Center, string.Format("$msg_added {0}", item.m_shared.m_name));
        user.GetInventory().RemoveItem(item, 1);
        nview.InvokeRPC("InputUsed", item.m_dropPrefab.name);
        return true;
    }

    protected void RPC_InputUsed(long sender, string itemName)
    {
        if (!nview.IsOwner()) return;

        SetItem(counter, itemName);
        counter++;
    }
}