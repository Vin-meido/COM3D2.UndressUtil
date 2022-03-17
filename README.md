
# COM3D2.UndressUtil

Enables the use of the Undressing window in the ADV, Recollection, Scout, and Guest modes. This is (almost) the same UI used for undressing in Dance mode.

Uses COM3D2's UI system so the plugin can be used in VR mode (unlike other plugins that use IMGUI).

[![demo](https://i.gyazo.com/dea45d459e4cd24c11dda277db056d24.png)](https://i.gyazo.com/e72ee1a75a3486af8181aa3c9914d719.mp4)

This plugin works with all versions of COM3D2:

- EN 1.13 or newer
- JP 2.13 or newer
- COM3D2.5 3.13 X0 or newer


## Known issues

(EN and JP COM3D2 only) Minor stuttering may occur when switching into Half Undress mode for the first time in a scene. Subsequent switching in and out of the mode should be fine however. This will be further optimized in a later update.


## Requirements

This plugin requires COM3D2API. If you use CMI, then this plugin should be included in your install already. If not, you can get it from (https://github.com/DeathWeasel1337/COM3D2_Plugins).


## Installation

**Important**: UndressUtil is now available via CMI (https://github.com/krypto5863/COM-Modular-Installer). Use the CMI installer to add the plugin into your game. However, if you wish to update manually, or if you are not using CMI to manage your plugins, you can do the following:

1. Make sure you have your game updated to latest version

2. Copy COM3D2.UndressUtil.dll in BepInEx/plugins folder of your COM3D2 installation

3. Run the game. Once in the game, hit `F1` to configure shortcut (when using in non VR)


## Configuration

These settings can be changed via Configuration Manager (comes with COM Modular Installer):

**General**

- Auto show in VR: Automatically show the undress window when a maid is present even when in VR.

- Auto show outside VR: Automatically show the undress window when a maid is present even when not in VR.

- Auto hide: Automatically hides the undress window when there are no visible maids in the scene

- Shortcut: shortcut key to use to show the undress window manually (unset by default)

- Keep yotogi undress state: When in yotogi, keep undress state when switching to a new position / skill. Both for normal yotogi and free/recollection mode.

- Verbose log: enable verbose logging in BepInEx console. Enable only when you want to report a problem.

**Scene settings**

By default, if Auto show is enabled for the particular mode (VR or non-VR), the undress window would only be displayed in ADV, Recollection, Guest, and Scout scenes. You can enable the following additional settings to alter this behaviour:

- Auto show in all scenes: Show in all scenes (disabled by default). Note that this may conflict with undressing functions native to that scene (e.g. Dance mode and/or Maid Edit mode)

- Auto show in yotogi: Show in yotogi scenes (disabled by default). Yotogi scenes normally have their own undressing panel. However the undress window can give you finer control, support half undress, and as well as undress the master (for COM3D2.5 X0)
