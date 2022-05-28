using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.UndressUtil.Plugin.UIHelper
{
    static class UIUtils
    {
        static UIAtlas atlasCommon;

        public static UIAtlas GetAtlasCommon()
        {
            if (atlasCommon == null)
            {
                var prefab = Resources.Load<UIAtlas>("CommonUI/Atlas/AtlasCommon");
                atlasCommon = GameObject.Instantiate(prefab);
                GameObject.DontDestroyOnLoad(atlasCommon);
            }
            return atlasCommon;
        }

        public static Font GetNotoSansCJKjpDemiLightFont()
        {
            return Resources.Load<Font>("font/notosanscjkjp-hinted/notosanscjkjp-demilight");
        }
    }
}
