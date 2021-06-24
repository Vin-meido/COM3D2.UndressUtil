using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Configuration;

namespace COM3D2.UndressUtil.Plugin
{
    public class UndressUtilConfig
    {
        public ConfigEntry<bool> verboseLog;
        public ConfigEntry<bool> autoShowInNonVr;
        public ConfigEntry<KeyboardShortcut> showShortcut;

        public UndressUtilConfig(ConfigFile conf)
        {
            showShortcut = conf.Bind(
                "General",
                "showShortcut",
                KeyboardShortcut.Empty,
                "Keyboard shortcut to use to show the undressing window");

            autoShowInNonVr = conf.Bind(
                "General",
                "autoShowInNonVr",
                true,
                "Automatically show undress window when in non VR mode. Window is always shown when in VR mode.");

            verboseLog = conf.Bind(
                "General",
                "verboseLog",
                false,
                "Enable verbose logging");
        }

    }
}
