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
        public ConfigEntry<bool> autoShowInVr;
        public ConfigEntry<bool> autoShowInNonVr;
        public ConfigEntry<bool> autoHide;
        public ConfigEntry<bool> autoShowInYotogi;
        public ConfigEntry<bool> autoShowInAllScenes;
        public ConfigEntry<bool> keepYotogiUndressState;
        public ConfigEntry<KeyboardShortcut> showShortcut;

        public UndressUtilConfig(ConfigFile conf)
        {
            showShortcut = conf.Bind(
                "General",
                "Shortcut",
                KeyboardShortcut.Empty,
                "Keyboard shortcut to use to show the undressing window");

            autoShowInVr = conf.Bind(
                "General",
                "Auto show in VR",
                false,
                "Automatically show undress window when in VR mode");

            autoShowInNonVr = conf.Bind(
                "General",
                "Auto show outside VR",
                false,
                "Automatically show undress window when in non VR mode.");

            autoHide = conf.Bind(
                "General",
                "Auto hide",
                false,
                "Automatically hide window when no maids are active.");

            autoShowInYotogi = conf.Bind(
                "Scene settings",
                "Auto show in yotogi",
                false,
                "Automatically show undress window in yotogi scenes.");

            keepYotogiUndressState = conf.Bind(
                "Scene settings",
                "Keep yotogi undress state",
                false,
                "Keeps undress state in between yotogi skills.");

            autoShowInAllScenes = conf.Bind(
                "Scene settings",
                "Auto show in all scenes",
                false,
                "Automatically show in all scenes.");

            verboseLog = conf.Bind(
                "General",
                "Verbose log",
                false,
                "Enable verbose logging");
        }

    }
}
