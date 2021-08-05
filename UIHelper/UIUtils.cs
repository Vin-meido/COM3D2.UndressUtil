using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.UndressUtil.Plugin.UIHelper
{
    static class UIUtils
    {
        static readonly Dictionary<string, UIAtlas> atlasCache = new Dictionary<string, UIAtlas>();

        public static UIAtlas GetAtlas(string name)
        {
            if(atlasCache.ContainsKey(name))
            {
                return atlasCache[name];
            }

            var obj = Resources.FindObjectsOfTypeAll<UIAtlas>()
                .Where(o => o.name == name)
                .FirstOrDefault();

            if (obj != null)
            {
                var cachedObj = GameObject.Instantiate(obj);
                GameObject.DontDestroyOnLoad(cachedObj);
                atlasCache[name] = cachedObj;
                return cachedObj;
            }

            return null;
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
