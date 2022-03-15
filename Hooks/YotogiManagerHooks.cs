using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Events;
using HarmonyLib;

namespace COM3D2.UndressUtil.Plugin.Hooks
{
    public class YotogiManagerHooks
    {

        public static readonly UnityEvent OnYotogiStart = new UnityEvent();
        public static readonly UnityEvent OnPreYotogiStart = new UnityEvent();

        static Harmony harmony;

        public static void Init()
        {
            if (harmony == null)
            {
                harmony = Harmony.CreateAndPatchAll(typeof(YotogiManagerHooks));
                Log.LogVerbose("YotogiManagerHooks loaded");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(WfScreenManager), "RunScreen")]
        public static void YotogiManager_RunScreen(string screen_name, object __instance)
        {
            if (__instance is YotogiManager || __instance is YotogiOldManager)
            {
                Log.LogVerbose($"YotogiManager_RunScreen({screen_name})");
                //Instance.OnRunScreen(screen_name);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(
            typeof(YotogiPlayManager),
            nameof(YotogiPlayManager.OnCall))]
        public static void YotogiPlayManager_OnCall_Prefix()
        {
            Log.LogVerbose("Yotogi setup, disabling maid update notifications");
            MaidHooks.IncreaseSupressStack();
            try
            {
                OnPreYotogiStart.Invoke();
            }
            catch (Exception e)
            {
                Log.LogError(e);
            }
        }

        [HarmonyFinalizer]
        [HarmonyPatch(
            typeof(YotogiPlayManager),
            nameof(YotogiPlayManager.OnCall))]
        public static void YotogiPlayManager_OnCall_Finalizer()
        {
            Log.LogVerbose("Yotogi setup complete");
            MaidHooks.DecreaseSupressTack();
            try
            {
                OnYotogiStart.Invoke();
            }
            catch(Exception e)
            {
                Log.LogError(e);
            }
        }

    }
}
