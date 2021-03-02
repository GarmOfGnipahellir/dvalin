using HarmonyLib;

namespace Dvalin.Patches
{
    namespace Player
    {
        [HarmonyPatch(typeof(global::Player), "OnSpawned")]
        class OnSpawned
        {
            public delegate void PostfixHandler(global::Player player);

            public static event PostfixHandler PostfixEvent;

            static void Postfix(global::Player __instance)
            {
                Logger.LogInfoFormat("{0} spawned.", __instance);

                PostfixEvent?.Invoke(__instance);
            }
        }
    }

    namespace Smelter
    {
        [HarmonyPatch(typeof(global::Smelter), "Awake")]
        class Awake
        {
            public delegate void PostfixHandler(global::Smelter smelter);

            public static event PostfixHandler PostfixEvent;

            static void Postfix(global::Smelter __instance)
            {
                PostfixEvent?.Invoke(__instance);
            }
        }
    }

    namespace Localization
    {
        [HarmonyPatch(typeof(global::Localization), "SetupLanguage")]
        class SetupLanguage
        {
            public delegate void PostfixHandler(string language);

            public static event PostfixHandler PostfixEvent;

            static void Postfix(string language)
            {
                PostfixEvent?.Invoke(language);
            }
        }

        [HarmonyPatch(typeof(global::Localization), "Translate")]
        class Translate
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

    namespace ZNetScene
    {
        [HarmonyPatch(typeof(global::ZNetScene), "Awake")]
        class Awake
        {
            public delegate void PrefixHandler(global::ZNetScene nscene);

            public static event PrefixHandler PrefixEvent;

            static void Prefix(global::ZNetScene __instance)
            {
                Logger.LogInfoFormat("{0} awoken.", __instance);

                PrefixEvent?.Invoke(__instance);
            }
        }
    }

    namespace ObjectDB
    {
        [HarmonyPatch(typeof(global::ObjectDB), "Awake")]
        class Awake
        {
            public delegate void PrefixHandler(global::ObjectDB objectDb);

            public static event PrefixHandler PrefixEvent;

            static void Prefix(global::ObjectDB __instance)
            {
                Logger.LogInfoFormat("{0} awoken.", __instance);

                PrefixEvent?.Invoke(__instance);
            }
        }
    }
}