using System.IO;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;

[BepInPlugin(k_Guid, k_Name, k_Version)]
[BepInProcess("valheim.exe")]
public class Plugin : BaseUnityPlugin
{
    public const string k_Guid = "com.garmofgnipahellir.dwarvenindustry";
    public const string k_Name = "Dwarven Industry";
    public const string k_Version = "0.1.0";

    public Dvalin.Main dvalin;

    void Awake()
    {
        dvalin = new Dvalin.Main();

        Dvalin.Patches.Player.UpdatePlacementGhost.PostfixEvent += UpdateMachinePlacementGhost;
    }

    void OnDestroy()
    {
        dvalin.Destroy();
    }

    void UpdateMachinePlacementGhost(Player player)
    {
        GameObject placementGhost = typeof(Player)
            .GetField("m_placementGhost", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(player) as GameObject;
        if (placementGhost == null) return;

        Machine machine = placementGhost.GetComponent<Machine>();
        if (machine == null) return;

        machine.cableMount.ReconnectInRange();
    }
}