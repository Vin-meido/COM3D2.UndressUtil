using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace COM3D2.UndressUtil.Plugin.UIHelper
{
    public static class ItemIconBuilder
    {
        public static GameObject Build(GameObject parent)
        {
            var texture = NGUITools.AddWidget<UITexture>(parent);
            texture.width = 70;
            texture.height = 70;
            texture.pivot = UIWidget.Pivot.Center;

            NGUITools.AddWidgetCollider(texture.gameObject);

            texture.GetOrAddComponent<UIButton>();

            return texture.gameObject;
        }
    }
}
