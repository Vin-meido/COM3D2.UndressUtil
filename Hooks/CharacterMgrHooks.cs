using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using HarmonyLib;


namespace COM3D2.UndressUtil.Plugin.Hooks
{

    [HarmonyPatch(typeof(CharacterMgr))]
    class CharacterMgrHooks
    {
        public static readonly MaidEvent MaidActivated = new MaidEvent();
        public static readonly MaidEvent MaidDeactivated = new MaidEvent();

        static Harmony harmony;

        public static bool supress { get; set; } = false;

        [HarmonyPrefix]
        [HarmonyPatch(nameof(CharacterMgr.CharaVisible))]
        static void CharaVisible(int f_nActiveSlot, bool f_bVisible, bool f_bMan, CharacterMgr __instance)
        {
            if (!supress && !f_bMan)
            {
                var maid = __instance.GetMaid(f_nActiveSlot);
                if (maid != null)
                {
                    Log.LogVerbose("CharacterMgr.CharaVisble {0} => {1}", maid, f_bVisible);
                    Log.LogVerbose(Environment.StackTrace);
                    if (f_bVisible)
                    {
                        MaidActivated.Invoke(maid);
                    }
                    else
                    {
                        MaidDeactivated.Invoke(maid);
                    }

                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(CharacterMgr.Deactivate))]
        static void Deactivate(int f_nActiveSlotNo, bool f_bMan, CharacterMgr __instance)
        {
            if (!supress && !f_bMan)
            {
                var maid = __instance.GetMaid(f_nActiveSlotNo);
                if (maid != null)
                {
                    Log.LogVerbose("CharacterMgr.Deactivate {0}", maid);
                    Log.LogVerbose(Environment.StackTrace);
                    MaidDeactivated.Invoke(maid);

                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(CharacterMgr.SetActiveMaid))]
        static void SetActiveMaid(Maid f_maid, int f_nActiveSlotNo)
        {
            if(!supress && f_maid != null)
            {
                Log.LogVerbose("CharacterMgr.SetActiveMaid {0}", f_maid);
                Log.LogVerbose(Environment.StackTrace);
                MaidActivated.Invoke(f_maid);
            }
        }

        public static void Init()
        {
            if (harmony == null)
            {
                harmony = new Harmony("org.bepinex.plugins.com3d2.undressutil.hooks.CharacterMgrHooks");
                harmony.PatchAll(typeof(CharacterMgrHooks));
                Log.LogVerbose("CharacterMgr hooks loaded");
            }
        }
    }
}
