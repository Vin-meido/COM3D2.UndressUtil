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
        public ConfigEntry<bool> disableSceneRestrictions;
        public ConfigEntry<KeyboardShortcut> showShortcut;

        public UndressUtilConfig(ConfigFile conf)
        {
            showShortcut = conf.Bind(
                "General",
                "Shortcut",
                KeyboardShortcut.Empty,
                "Keyboard shortcut to use to show the undressing window");

            autoShowInNonVr = conf.Bind(
                "General",
                "Auto show outside VR",
                true,
                "Automatically show undress window when in non VR mode. Window is always shown when in VR mode.");

            disableSceneRestrictions = conf.Bind(
                "General",
                "Disable scene restrictions",
                false,
                "Allow using undress window regardless of scene type");

            verboseLog = conf.Bind(
                "General",
                "Verbose log",
                false,
                "Enable verbose logging");
        }

    }
}
