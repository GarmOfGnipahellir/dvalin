using System.Reflection;
using System.Collections.Generic;
using System.IO;
using Dvalin.Core;
using HarmonyLib;
using UnityEngine;

namespace Dvalin.Patches
{
    [HarmonyPatch(typeof(Player), "OnSpawned")]
    class PlayerOnSpawned
    {
        public delegate void PostfixHandler(Player player);

        public static event PostfixHandler PostfixEvent;

        static void Postfix(Player __instance)
        {
            DvalinLogger.LogInfoFormat("{0} spawned.", __instance);

            PostfixEvent?.Invoke(__instance);
        }
    }

    [HarmonyPatch(typeof(ZNetScene), "Awake")]
    class ZNetSceneAwake
    {
        public delegate void PrefixHandler(ZNetScene zNetScene);

        public static event PrefixHandler PrefixEvent;

        static void Prefix(ZNetScene __instance)
        {
            DvalinLogger.LogInfoFormat("{0} awoken.", __instance);

            PrefixEvent?.Invoke(__instance);
        }
    }

    [HarmonyPatch(typeof(Localization), "SetupLanguage")]
    class LocalizationSetupLanguagePatch
    {
        public delegate void PostfixHandler(string language);

        public static event PostfixHandler PostfixEvent;

        static void Postfix(string language)
        {
            PostfixEvent.Invoke(language);
        }
    }

    [HarmonyPatch(typeof(Localization), "Translate")]
    class LocalizationTranslatePatch
    {
        public delegate bool PostfixHandler(string word, out string translated);

        public static event PostfixHandler PostfixEvent;

        static string Postfix(string result, string word)
        {
            string failed = string.Format("[{0}]", word);
            if (result != failed)
            {
                return result;
            }

            if (PostfixEvent.Invoke(word, out string translated))
            {
                return translated;
            }

            return failed;
        }
    }
}
