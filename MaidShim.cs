using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace COM3D2.UndressUtil.Plugin
{
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
            }
        }



    }

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

}
