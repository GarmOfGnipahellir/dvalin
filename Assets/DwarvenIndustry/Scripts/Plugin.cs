using System.IO;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using HarmonyLib;

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
    }

    void OnDestroy()
    {
        dvalin.Destroy();
    }
}