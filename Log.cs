using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;

namespace COM3D2.UndressUtil.Plugin
{
    internal class Log
    {
        internal static ManualLogSource Logger
        {
            get
            {
                return UndressUtilPlugin.Instance.Logger;
            }
        }

        internal static bool enableVerbose
        {
            get
            {
                return UndressUtilPlugin.Instance.Config.verboseLog.Value;
            }
        }

        internal static void LogInfo(string message, params object[] args)
        {
            Logger.LogInfo(string.Format(message, args));
        }

        internal static void LogError(string message, params object[] args)
        {
            Logger.LogError(string.Format(message, args));
        }

        internal static void LogVerbose(string message, params object[] args)
        {
            if (!enableVerbose) return;
            Logger.LogInfo(string.Format(message, args));
        }
    }
}
