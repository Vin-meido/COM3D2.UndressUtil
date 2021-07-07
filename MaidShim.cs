using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine.Events;

namespace COM3D2.UndressUtil.Plugin
{
    [HarmonyPatch(typeof(BaseKagManager))]
    class BaseKagManagerShim
    {
        private Harmony harmony;

        public MaidEvent TagItemSetEvent { get; private set; } = new MaidEvent();

        public static BaseKagManagerShim Instance { get; private set; }

        public static void Init()
        {
            if (Instance == null)
            {
                Instance = new BaseKagManagerShim();
            }
        }

        protected BaseKagManagerShim()
        {
            harmony = new Harmony("org.bepinex.plugins.com3d2.undressutil.basekagmanagershim");
            harmony.PatchAll(typeof(BaseKagManagerShim));
            Log.LogVerbose("BaseKagManager patch loaded");
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(BaseKagManager.TagItemSet))]
        public static void TagItemSet(KagTagSupport tag_data, BaseKagManager __instance)
        {
            var maid = __instance.GetMaidAndMan(tag_data, true);
            if (maid != null)
            {
                Log.LogVerbose("BaseKagManager.TagItemSet for maid {0}", maid);
                Instance.TagItemSetEvent.Invoke(maid);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(BaseKagManager.TagItemReset))]
        public static void TagItemReset(KagTagSupport tag_data, BaseKagManager __instance)
        {
            var maid = __instance.GetMaidAndMan(tag_data, true);
            if (maid != null)
            {
                Log.LogVerbose("BaseKagManager.TagItemReset for maid {0}", maid);
                Instance.TagItemSetEvent.Invoke(maid);
            }
        }
    }


    [HarmonyPatch(typeof(Maid))]
    class MaidShim
    {
        private static Harmony harmony;

        public static void Init()
        {
            if (harmony == null)
            {
                harmony = new Harmony("org.bepinex.plugins.com3d2.undressutil.maidshim");
                harmony.PatchAll(typeof(MaidShim));
                Log.LogVerbose("MaidShim patch loaded");
            }
        }

        /*

        [HarmonyPrefix]
        [HarmonyPatch("SetProp")]
        public static void SetProp1(MPN idx, int val, bool f_bTemp, Maid __instance)
        {
            Log.LogVerbose("Detected set prop {0}", __instance);
        }
        */

        [HarmonyPrefix]
        [HarmonyPatch("SetProp")]
        [HarmonyPatch(new Type[] { typeof(MaidProp) })]
        public static void SetProp(MaidProp mps, Maid __instance) {
            Log.LogVerbose("Detected set prop {0}", __instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch("SetProp")]
        [HarmonyPatch(new Type[] { typeof(MPN), typeof(string), typeof(int), typeof(bool), typeof(bool)})]
        public static void SetProp(MPN idx, string filename, int f_nFileNameRID, bool f_bTemp, bool f_bNoScale, Maid __instance)
        {
            
            Log.LogVerbose("Detected set prop {0}", __instance);

        }


        [HarmonyPrefix]
        [HarmonyPatch("AllProcProp")]
        public static void AllProcProp(Maid __instance)
        {
            //Log.LogVerbose("AllProcProp Detected prop reload {0}\n{1}", __instance, Environment.StackTrace);

        }

        [HarmonyPrefix]
        [HarmonyPatch("AllProcPropSeqStart")]
        public static void AllProcPropSeqStart(Maid __instance)
        {
            //Log.LogVerbose("AllProcPropSeqStart Detected prop reload {0}\n{1}", __instance, Environment.StackTrace);
        }

        [HarmonyPrefix]
        [HarmonyPatch("SetProp")]
        [HarmonyPatch(new Type[] { typeof(string), typeof(string), typeof(int), typeof(bool), typeof(bool) })]
        public static void SetProp(string tag, string filename, int f_nFileNameRID, bool f_bTemp = false, bool f_bNoScale = false, Maid __instance=null)
        {
            Log.LogVerbose("SetProp Detected prop set {0}\n" +
                "tag:      {1}\n" +
                "filename: {2}\n" +
                "rid:      {3}\n" +
                "temp:     {4}\n" +
                "noscale:  {5}", __instance, tag, filename, f_nFileNameRID, f_bTemp, f_bNoScale);
        }


        /*
        [HarmonyPrefix]
        [HarmonyPatch("SetPropIn")]
        [HarmonyPatch(new Type[] { typeof(MaidProp), typeof(string), typeof(int), typeof(bool), typeof(bool) })]
        public static void SetPropIn(MaidProp mp, string filename, int f_nFileNameRID, bool f_bTemp, bool f_bNoScale = false, Maid __instance=null)
        {
            Log.LogVerbose("Detected SetPropIn {0}", __instance);
        }


        /*
        [HarmonyPrefix]
        [HarmonyPatch("SetPropIn")]
        [HarmonyPatch(new Type[] { typeof(MPN), typeof(int), typeof(bool) })]
        public static void SetPropIn(MPN idx, int val, bool f_bTemp = false, Maid __instance = null)
        {
            Log.LogVerbose("Detected SetPropIn {0}", __instance);

        }


        /*
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Maid.SetProp))]
        [HarmonyPatch(new Type[] { typeof(MPN), typeof(string), typeof(int), typeof(bool)})]
        public static void SetProp3(MPN idx, string filename, int f_nFileNameRID, bool f_bTemp, Maid __instance)
        {
            Log.LogVerbose("Detected set prop {0}", __instance);

        }
        */
    }

    /*
    [HarmonyPatch(typeof(NDebug))]
    class NDebugShim
    {
        private static Harmony harmony;

        public static void Init()
        {
            if (harmony == null)
            {
                harmony = new Harmony("org.bepinex.plugins.com3d2.undressutil.NDebugShim");
                Log.LogVerbose("Patching NDebug.MessageBox");
                harmony.PatchAll(typeof(NDebugShim));
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("MessageBox")]
        public static bool MessageBox(string f_strTitle, string f_strMsg)
        {
            Log.LogVerbose("Supressing message box {0}", f_strTitle);
            return false;
        }
    }
    */

}
