
# COM3D2.UndressUtil

Enables the use of the Undressing window in the ADV, Recollection, Scout, and Guest modes. This is (almost) the same UI used for undressing in Dance mode.

Uses COM3D2's UI system so the plugin can be used in VR mode (unlike other plugins that use IMGUI).

[![demo](https://i.gyazo.com/dea45d459e4cd24c11dda277db056d24.png)](https://i.gyazo.com/e72ee1a75a3486af8181aa3c9914d719.mp4)

This plugin works with all versions of COM3D2:

- EN 1.13
- JP 2.13
- COM3D2.5 3.13 X0

## Installation

1. Make sure you have your game updated to latest version

2. If you havent yet, get and install COM Moduler Installer (https://github.com/krypto5863/COM-Modular-Installer).

3. Copy COM3D2.UndressUtil.dll in BepInEx/plugins folder of your COM3D2 installation

4. Run the game. Once in the game, hit `F1` to configure shortcut (when using in non VR)


## Configuration

These settings can be changed via Configuration Manager (comes with COM Modular Installer):

- Auto hide: Automatically hides the undress window when there are no visible maids in the scene

- Auto show outside VR: Automatically show the undress window when a maid is present even when not in VR. (The window is shown automatically when in VR mode).

- Disable scene restrictions: Remove restrictions on which scene types the undress window can be shown for.

- Shortcut: shortcut key to use to show the undress window manually

- Verbose log: enable verbose logging in BepInEx console. Enable only when you want to report a problem.



## Build from source

You shouldnt need to, unless you want to modify the functionality. If you do, clone this git repository then copy the following dll files from your COM3D2 game into the libs directory:

From BepInEx\Core:
- 0Harmony.dll
- BepInEx.dll

From COM3D2x64_Data\Managed:
- Assembly-CSharp.dll
- Assembly-CSharp-firstpass.dll
- UnityEngine.dll

Install VS Studio community. See the general programming resources in the Custom Maid Server on how to install and which packages to include.

If you are using the EN version of the game to compile, select "en_jp-2.0" build configuration (as the en version dlls do not contain some of functions.) Though dlls compiled against the jp version should work with en version of the game.

Open the sln file, and build!
