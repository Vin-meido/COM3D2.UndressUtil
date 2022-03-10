﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine.Events;

using HarmonyLib;

namespace COM3D2.UndressUtil.Plugin.Hooks
{
    public static class MaidHooks
    {
		public static readonly MaidEvent OnMaidPropUpdate = new MaidEvent();

		static Harmony harmony;
		static bool supress = false;

		public static void Init()
		{
			if (harmony == null)
			{
				harmony = Harmony.CreateAndPatchAll(typeof(MaidHooks));
				Log.LogVerbose("Maid hooks loaded");
			}
		}


		[HarmonyPatch(typeof(Maid), "AllProcProp")]
		[HarmonyPatch(typeof(Maid), "AllProcPropSeq")]
		[HarmonyPostfix]
		private static void Maid_AllProcProp(Maid __instance)
		{
			if (supress) return;

#if DEBUG
			Log.LogVerbose("Detected prop load for {0}", __instance);
#endif

			try
            {
				OnMaidPropUpdate.Invoke(__instance);
			}
            catch (Exception e)
            {
				Log.LogError(e);
            }
		}

		[HarmonyPatch(typeof(TBody), "FixMaskFlag")]
		[HarmonyPostfix]
		private static void TBody_FixMaskFlag(ref TBody __instance)
		{
			if (supress) return;

#if (DEBUG)
			Log.LogVerbose("Detected visibility change for {0}", __instance.maid);
#endif

			try
			{
				OnMaidPropUpdate.Invoke(__instance.maid);
			}
			catch (Exception e)
			{
				Log.LogError(e);
			}
		}

		public static void Supress(Action action)
        {
#if DEBUG
			Log.LogVerbose("Supressing proc events");
#endif

			supress = true;
			try
            {
				action();
            }
			finally
            {
#if DEBUG
				Log.LogVerbose("Action complete, resuming proc events");
#endif

				supress = false;
            }
        }
	}

}
