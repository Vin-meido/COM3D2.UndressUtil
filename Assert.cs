using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.UndressUtil.Plugin
{
    class Assert
    {
        public static bool raiseExceptions = true;

        public static void IsNotNull(object obj, string message, params object[] args)
        {
            if (obj == null)
            {
                LogError(message, args);
            }
        }

        public static void IsNotEmpty(string str, string message, params object[] args)
        {
            if (str == "")
            {
                LogError(message, args);
            }
        }

        private static void LogError(string message, object[] args)
        {
            if (args.Length > 0)
            {
                message = String.Format(message, args);
            }

            UndressUtilPlugin.Instance.LogError(message);
            if (raiseExceptions)
            {
                throw new Exception(message);
            }
        }
    }
}
