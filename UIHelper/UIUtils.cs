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
        static UIAtlas atlasDance;

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

        public static UIAtlas GetRhythmActionUIAtlas()
        {
            if (atlasDance == null)
            {
                var prefab = Resources.Load<UIAtlas>("scenedance/rhythm_action/sprite/rhythmactionui");
                atlasDance = GameObject.Instantiate(prefab);
                GameObject.DontDestroyOnLoad(atlasDance);
            }

            return atlasDance;
        }

        public static Font GetNotoSansCJKjpDemiLightFont()
        {
            return Resources.Load<Font>("font/notosanscjkjp-hinted/notosanscjkjp-demilight");
        }

        public static Sprite GetVRSliderCursorSprite()
        {
            return Resources.Load<Sprite>("scenevrcommunication/tablet/sprite/system ui/vr_slider_cursor");
        }
    }
}
