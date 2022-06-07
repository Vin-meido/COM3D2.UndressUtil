using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using COM3D2.UndressUtil.Plugin.UIHelper;

namespace COM3D2.UndressUtil.Plugin
{
    class Prefabs
    {
        public static GameObject CreateUndressWindow(GameObject parent)
        {
            var instance = UndressWindowBuilder.Build(parent);

            // Setup maid status tracking
            instance.AddComponent<MaidTracker>();

            // Setup new handler
            instance.AddComponent<UndressWindowManager>();

            // Set default name
            instance.name = "UndressWindow";

            // Add widget
            var itemWindowTransform = instance.transform.Find("ItemWindow");
            Assert.IsNotNull(itemWindowTransform, "Could not find ItemWindow");
            CreateHalfUndressWidget(itemWindowTransform.gameObject);
            CreateRefreshWidget(itemWindowTransform.gameObject);


            return instance;
        }

        public static GameObject CreateHalfUndressWidget(GameObject parent)
        {
            var obj = UIUtils.GetAtlasCommon();
            Assert.IsNotNull(obj, "Cannot find AtlasCommon");
            var atlas = obj.GetComponent<UIAtlas>();

            var button = Button.Add(parent, atlas, "cm3d2_common_plate_white");
            var go = button.gameObject;
            go.name = "HalfUndressButton";
            button.size = new Vector2(70, 70);
            button.position = new Vector2(-170, 377);
            button.label = "Half undress mode";
            button.backgroundColor = Color.gray;
            return button.gameObject;
        }

        public static GameObject CreateRefreshWidget(GameObject parent)
        {
            var obj = UIUtils.GetAtlasCommon();
            Assert.IsNotNull(obj, "Cannot find AtlasCommon");
            var atlas = obj.GetComponent<UIAtlas>();

            var button = Button.Add(parent, atlas, "cm3d2_common_plate_white");
            var go = button.gameObject;
            go.name = "RefreshButton";
            button.size = new Vector2(70, 70);
            button.position = new Vector2(-170, 300);
            button.label = "Refresh";
            return button.gameObject;
        }

        public static GameObject CreateMaidIcon(GameObject parent)
        {
            var instance = MaidIconBuilder.Build(parent);

            // setup new handler
            instance.AddComponent<MaidIcon>();

            return instance;
        }

        public static GameObject CreateItemIcon(GameObject parent)
        {
            var instance = ItemIconBuilder.Build(parent);
            instance.AddComponent<UndressItemManager>();
            return instance;
        }
    }
}
