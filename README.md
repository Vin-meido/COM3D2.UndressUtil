
# COM3D2.UndressUtil

Enables the use of the Undressing window in the ADV, Recollection, Scout, and Guest modes.

Uses COM3D2's UI system so the plugin can be used in VR mode (unlike other plugins that use IMGUI).

[![demo](https://i.gyazo.com/dea45d459e4cd24c11dda277db056d24.png)](https://i.gyazo.com/e72ee1a75a3486af8181aa3c9914d719.mp4)

## Status

Currently a work in progress

## Build from source

Copy the following dll files from your COM3D2 game into the libs directory:

From BepInEx\Core:
- 0Harmony.dll
- BepInEx.dll

From COM3D2x64_Data\Managed:
- Assembly-CSharp.dll
- Assembly-CSharp-firstpass.dll
- Assembly-UnityScript-firstpass.dll
- UnityEngine.dll

Install VS Studio community. See the general programming resources in the Custom Maid Server on how to install and which packages to include.

Open the sln file, and build!
