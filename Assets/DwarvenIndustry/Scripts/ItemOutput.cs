using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// NOTE: works more like a queue than just a container right now
// should really only keep track of how many of each item it has not
// an ordered list of item names. side effect of basing it on input
// HOLUP! a queue is probably better when we get logistics in,
// since they should output in the same order as they were added
// even if there's a bottle neck making it pile up.
public class ItemOutput : MonoBehaviour
{
    private const string k_ZDONamespace = "output_";

    public int capacity = 10;

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
            return zdo.GetInt(k_ZDONamespace + nameof(counter));
        }
        set
        {
            zdo.Set(k_ZDONamespace + nameof(counter), value);
        }
    }

    void Awake()
    {
        useSwitch.m_onUse += OnOutputUsed;

        nview.Register("OutputUsed", RPC_OutputUsed);
    }

    void FixedUpdate()
    {
        if (!nview.IsValid()) return;

        UpdateHoverTexts();
    }

    public string GetItem(int index) { return zdo.GetString(string.Format("{0}item{1}", k_ZDONamespace, index)); }
    public void SetItem(int index, string value) { zdo.Set(string.Format("{0}item{1}", k_ZDONamespace, index), value); }

    public void AddItem(string itemName, int num = 1)
    {
        for (int i = 0; i < num; i++)
        {
            SetItem(counter, itemName);
            counter++;
        }
    }
    public void AddItem(ItemDrop.ItemData item, int num = 1) { AddItem(item.m_dropPrefab.name, num); }

    protected void UpdateHoverTexts()
    {
        useSwitch.m_hoverText = string.Format(
            // NOTE: $piece_smelter_additem translation might change!
            "{0} ({1}/{2})\n[<color=yellow><b>$KEY_Use</b></color>] $piece_smelter_empty",
            piece.m_name,
            counter,
            capacity
        );

        for (int i = 0; i < counter; i++)
        {
            useSwitch.m_hoverText += "\n" + GetItem(i);
        }
    }

    protected bool OnOutputUsed(Switch sw, Humanoid user, ItemDrop.ItemData item)
    {
        nview.InvokeRPC("OutputUsed");
        return true;
    }

    protected void RPC_OutputUsed(long sender)
    {
        if (!nview.IsOwner()) return;

        var stacks = new Dictionary<string, int>();
        for (int i = 0; i < counter; i++)
        {
            var itemName = GetItem(i);
            var num = 0;
            stacks.TryGetValue(itemName, out num);
            stacks[itemName] = (num += 1);
        }
        foreach (var kvp in stacks)
        {
            var item = ObjectDB.instance.GetItemPrefab(kvp.Key);

            UnityEngine.Object.Instantiate<GameObject>(
                item,
                transform.position,
                transform.rotation
            ).GetComponent<ItemDrop>().m_itemData.m_stack = kvp.Value;
        }
        counter = 0;
    }
}