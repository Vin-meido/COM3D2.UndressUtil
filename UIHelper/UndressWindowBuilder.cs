using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;


namespace COM3D2.UndressUtil.Plugin.UIHelper
{
    public static class UndressWindowBuilder
    {

        public static GameObject NewGameObject(GameObject parent, string name, params Type[] components)
        {
            var obj = new GameObject(name, components);
            obj.layer = 8;
            obj.transform.SetParent(parent.transform, false);

            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;

            return obj;
        }

        public static T AddWidget<T>(GameObject go) where T : UIWidget
        {
            var component = go.AddComponent<T>();
            component.depth = NGUITools.CalculateNextDepth(go.transform.parent.gameObject);
            return component;
        }

        public static GameObject Build(GameObject root)
        {
            var undressWindow = NewGameObject(root, "UndressWindow", typeof(UIPanel));
            undressWindow.GetComponent<UIPanel>().depth = NGUITools.CalculateNextDepth(root);

            UnityEngine.Debug.Log($"Created gameobject {undressWindow}");

            var itemWindow = NewGameObject(undressWindow, "ItemWindow");

            CreateBG3(itemWindow);

            var bg = NewGameObject(itemWindow, "BG");

            CreateTitleBar(bg);

            CreateButton(
                "AllUndress",
                "Undress all",
                new Vector2(110, 50),
                new Vector3(-60f, 353f),
                bg);

            CreateButton(
                "AllDress",
                "Dress all",
                new Vector2(110, 50),
                new Vector3(60f, 353f),
                bg);

            CreateBG2(itemWindow);


            var itemGrid = NGUITools.AddChild<UIGrid>(itemWindow);
            itemGrid.name = "ItemGrid";
            itemGrid.arrangement = UIGrid.Arrangement.Horizontal;
            itemGrid.maxPerLine = 3;
            itemGrid.cellWidth = 80;
            itemGrid.cellHeight = 80;
            itemGrid.hideInactive = true;
            itemGrid.transform.localPosition = new Vector3(-78.85f, 278.9f);

            var maidGrid = NGUITools.AddChild<UIGrid>(itemWindow);
            maidGrid.name = "MaidIcon";
            maidGrid.arrangement = UIGrid.Arrangement.Vertical;
            maidGrid.maxPerLine = 4;
            maidGrid.cellWidth = 86;
            maidGrid.cellHeight = 86;
            maidGrid.transform.localPosition = new Vector3(-170f, 200f);

            return undressWindow;
        }

        private static void CreateBG2(GameObject itemWindow)
        {
            var bg2 = NGUITools.AddSprite(
                itemWindow,
                UIUtils.GetAtlasCommon(),
                "cm3d2_common_lineframe_white_d");
            bg2.name = "BG2";
            bg2.transform.localPosition = new Vector3(0f, 270f);
            bg2.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
            bg2.width = 258;
            bg2.height = 240;
        }

        private static void CreateButton(string name, string text, Vector2 size, Vector3 pos, GameObject parent)
        {
            var allUndressSprite = NGUITools.AddSprite(
                parent,
                UIUtils.GetAtlasCommon(),
                "cm3d2_common_plate_white");
            allUndressSprite.name = name;
            allUndressSprite.width = (int)size.x;
            allUndressSprite.height = (int)size.y;
            allUndressSprite.transform.localPosition = pos;
            NGUITools.AddWidgetCollider(allUndressSprite.gameObject);

            allUndressSprite.GetOrAddComponent<UIButton>();

            var label = NGUITools.AddWidget<UILabel>(allUndressSprite.gameObject);
            label.trueTypeFont = UIUtils.GetNotoSansCJKjpDemiLightFont();
            label.text = text;
            label.color = Color.black;
        }

        private static void CreateTitleBar(GameObject bg)
        {
            var titleBar = NewGameObject(bg, "TitleBar");
            var titleBarSprite = AddWidget<UISprite>(titleBar);
            titleBarSprite.atlas = UIUtils.GetAtlasCommon();
            titleBarSprite.spriteName = "cm3d2_common_plate_black_top_win";
            titleBarSprite.type = UIBasicSprite.Type.Sliced;
            titleBarSprite.width = 258;
            titleBarSprite.height = 20;
            titleBarSprite.pivot = UIWidget.Pivot.Top;
            titleBar.transform.localPosition = new Vector3(0, 410, 0);


            var titleBarText = NewGameObject(titleBar, "Text");
            var label = AddWidget<UILabel>(titleBarText);
            label.trueTypeFont = UIUtils.GetNotoSansCJKjpDemiLightFont();
            label.text = "Undressing mode";
            label.fontSize = 16;
            label.color = Color.white;
            label.width = 174;
            label.height = 20;
            label.pivot = UIWidget.Pivot.Left;
            label.alignment = NGUIText.Alignment.Left;
            titleBarText.transform.localPosition = new Vector3(-120.9f, -10f, 0f);

            var endButton = NewGameObject(titleBar, "End");
            endButton.AddComponent<BoxCollider>();

            var endSprite = AddWidget<UISprite>(endButton);
            endSprite.atlas = UIUtils.GetAtlasCommon();
            endSprite.spriteName = "cm3d2_common_win_btn_end";
            endSprite.width = endSprite.height = 16;
            endSprite.type = UIBasicSprite.Type.Sliced;
            endSprite.pivot = UIWidget.Pivot.Center;
            endSprite.ResizeCollider();

            endButton.AddComponent<UIButton>();

            endButton.transform.localPosition = new Vector3(115f, -10f);
        }

        private static void CreateBG3(GameObject itemWindow)
        {
            var bgsprite = NGUITools.AddSprite(
                itemWindow,
                UIUtils.GetAtlasCommon(),
                "cm3d2_common_plate_black");
            
            bgsprite.name = "BG3";
            bgsprite.width = 258;
            bgsprite.height = 260;
            bgsprite.depth = -2;

            NGUITools.AddWidgetCollider(bgsprite.gameObject);
            bgsprite.transform.localPosition = new Vector3(0, 280, 0);

            var dragger = bgsprite.GetOrAddComponent<PhotoWindowDragMove>();
            dragger.WindowTransform = itemWindow.transform.parent.transform;
        }
    }
}
