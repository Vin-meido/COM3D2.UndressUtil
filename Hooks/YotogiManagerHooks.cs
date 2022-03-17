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

#if DEBUG
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
#endif

#if DEBUG
        [HarmonyPostfix]
        [HarmonyPatch(
            typeof(UndressingManager),
            nameof(UndressingManager.ApplyAllMaid)
        )]
        public static void UndressingManager_ApplyAllMaid(UndressingManager __instance)
        {
            //Log.LogVerbose($"ApplyAllMaid:\n{Environment.StackTrace}");
        }
#endif

        [HarmonyPrefix]
        [HarmonyPatch(
            typeof(YotogiPlayManager),
            nameof(YotogiPlayManager.OnCall))]
        [HarmonyPatch(
            typeof(YotogiOldPlayManager),
            nameof(YotogiOldPlayManager.OnCall))]
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
        [HarmonyPatch(
            typeof(YotogiOldPlayManager),
            nameof(YotogiOldPlayManager.OnCall))]
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

#if DEBUG
        [HarmonyFinalizer]
        [HarmonyPatch(
            typeof(UndressingManager.UndressingData),
            nameof(UndressingManager.UndressingData.mask_mode),
            MethodType.Setter)]
        public static void UndressingManager_UndressingData_mask_mode_set(UndressingManager.UndressingData __instance)
        {
            if (__instance.unit_type == UndressingManager.UnitType.トップス)
            {
                Log.LogVerbose($"mask_mode {__instance.unit_type} => {__instance.mask_mode}:\n{Environment.StackTrace}");
            }

        }
#endif
    }
}
