using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace COM3D2.UndressUtil.Plugin.UIHelper
{
    public static class MaidIconBuilder
    {
        public static GameObject Build(GameObject parent)
        {
            var go = NGUITools.AddChild(parent);
            go.name = "MaidIcon(Clone)";

            var bg = NGUITools.AddWidget<UI2DSprite>(go);
            bg.name = "BG";
            bg.sprite2D = UIUtils.GetVRSliderCursorSprite();
            bg.depth = -1;
            bg.width = 70;
            bg.height = 70;

            var iconMask = NGUITools.AddChild<UIPanel>(go);
            iconMask.name = "IconMask";
            iconMask.depth = 14;
            iconMask.clipping = UIDrawCall.Clipping.TextureMask;
            iconMask.clipTexture = bg.sprite2D.texture;
            iconMask.sortingOrder = 8;
            iconMask.baseClipRegion = new Vector4(0, 0, 70, 70);

            var icon = NGUITools.AddWidget<UITexture>(iconMask.gameObject);
            icon.name = "Icon";
            icon.width = 70;
            icon.height = 70;
            icon.depth = 0;

            NGUITools.AddWidgetCollider(icon.gameObject);

            var frame = NGUITools.AddSprite(
                iconMask.gameObject,
                UIUtils.GetRhythmActionUIAtlas(),
                "AppelCoolRing");
            frame.name = "Frame";
            frame.width = 86;
            frame.height = 86;
            frame.depth = 6;
            frame.type = UIBasicSprite.Type.Sliced;
            frame.color = Color.clear;

            return go;
        }
    }
}
