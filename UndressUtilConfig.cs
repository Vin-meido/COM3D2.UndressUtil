using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Configuration;

namespace COM3D2.UndressUtil.Plugin
{
    public class UndressUtilConfig
    {
        public ConfigEntry<bool> autoShowInVr;
        public ConfigEntry<bool> verboseLog;

        public UndressUtilConfig(ConfigFile conf)
        {
            autoShowInVr = conf.Bind(
                "General",
                "autoShowInVr",
                true,
                "Automatically show when in VR mode");

            verboseLog = conf.Bind(
                "General",
                "verboseLog",
                false,
                "Enable verbose logging");
        }

    }
}
