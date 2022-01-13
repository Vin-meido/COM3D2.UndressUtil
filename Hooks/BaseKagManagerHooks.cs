using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using HarmonyLib;


namespace COM3D2.UndressUtil.Plugin.Hooks
{
    [HarmonyPatch(typeof(BaseKagManager))]
    class BaseKagManagerHooks
    {
        static Harmony harmony;

        public static readonly MaidEvent MaidActivated = new MaidEvent();
        public static readonly MaidEvent MaidDeactivated = new MaidEvent();
        public static readonly MaidEvent MaidPropUpdated = new MaidEvent();

        public static void Init()
        {
            if (harmony == null)
            {
                harmony = new Harmony("org.bepinex.plugins.com3d2.undressutil.hooks.basekagmanagerhooks");
                harmony.PatchAll(typeof(BaseKagManagerHooks));
                Log.LogVerbose("BaseKagManager hook loaded");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(BaseKagManager.TagItemReset))]
        [HarmonyPatch(nameof(BaseKagManager.TagItemSet))]
        [HarmonyPatch(nameof(BaseKagManager.TagPropSet))]
        [HarmonyPatch(nameof(BaseKagManager.TagPropReset))]
        [HarmonyPatch(nameof(BaseKagManager.TagItemMask))]
        [HarmonyPatch(nameof(BaseKagManager.TagItemMaskMode))]
        static void TagItemReset(KagTagSupport tag_data, BaseKagManager __instance, MethodBase __originalMethod)
        {
            var maid = BaseKagManager_GetMaid(__instance, tag_data);
            if (maid != null)
            {
                Log.LogVerbose("BaseKagManager.{1} for maid {0}", maid, __originalMethod.Name);
                MaidPropUpdated.Invoke(maid);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(BaseKagManager.TagCharaActivate))]
        static void TagCharaActivate(KagTagSupport tag_data, BaseKagManager __instance)
        {
            var maid = BaseKagManager_GetMaid(__instance, tag_data);
            if (maid)
            {
                Log.LogVerbose("BaseKagManager.TagCharaActivate {0}", maid);
                MaidActivated.Invoke(maid);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(BaseKagManager.TagCharaVisible))]
        static void TagCharaVisible(KagTagSupport tag_data, BaseKagManager __instance)
        {
            var maid = BaseKagManager_GetMaid(__instance, tag_data);
            var visible = tag_data.IsValid("true");
            if(maid)
            {
                Log.LogVerbose("BaseKagManager.TagCharaVisible {0} => {1}", maid, visible);
                if (visible)
                {
                    MaidActivated.Invoke(maid);
                } else
                {
                    MaidDeactivated.Invoke(maid);
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(BaseKagManager.TagCharaDeactivate))]
        static void TagCharaDeactivate(KagTagSupport tag_data, BaseKagManager __instance)
        {
            var maid = BaseKagManager_GetMaid(__instance, tag_data);
            if (maid)
            {
                Log.LogVerbose("BaseKagManager.TagCharaDeactivate {0}", maid);
                MaidDeactivated.Invoke(maid);
            }
        }

        static Maid BaseKagManager_GetMaid(BaseKagManager instance, KagTagSupport tag_data)
        {
            if (tag_data.IsValid("maid"))
            {
                var num = tag_data.GetTagProperty("maid").AsInteger();
                return instance.GetMaid(num);
            }

            return null;

        }

    }
}
