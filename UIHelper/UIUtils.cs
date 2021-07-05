using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.UndressUtil.Plugin.UIHelper
{
    static class UIUtils
    {
        public static UIAtlas GetAtlas(string name)
        {
            return Resources.FindObjectsOfTypeAll<UIAtlas>()
                .Where(o => o.name == name)
                .FirstOrDefault();
        }

        public static Font GetFont(string name)
        {
            return Resources.FindObjectsOfTypeAll<Font>()
                .Where(o => o.name == name)
                .FirstOrDefault();
        }

        public static AnimationClip GetAnimationClip(string name)
        {
            return Resources.FindObjectsOfTypeAll<AnimationClip>()
                .Where(o => o.name == name)
                .FirstOrDefault();
        }
    }
}
