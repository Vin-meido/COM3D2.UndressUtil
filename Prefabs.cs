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
            var ob = Resources.Load<GameObject>("SceneDance/Rhythm_Action/Prefab/RhythmAction_Mgr");
            var prefab = ob.transform.Find("DanceUndress").gameObject;

            var instance = UnityEngine.Object.Instantiate<GameObject>(prefab, parent.transform, false);
            //GameObject.Destroy(ob);

            // Remove default handlers
            GameObject.Destroy(instance.GetComponent<UndressDance_Mgr>());
            GameObject.Destroy(instance.GetComponentInChildren<UIWFTabPanel>());

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

            // Fix colliders
            var bgTransform = instance.transform.Find("ItemWindow/BG");
            Assert.IsNotNull(bgTransform, "Could not find BG");
            bgTransform.gameObject.GetComponent<UISprite>().ResizeCollider();

            var takeEventTransform = instance.transform.Find("TakeEvent");
            Assert.IsNotNull(takeEventTransform, "Could not find take event transform");
            GameObject.Destroy(takeEventTransform.gameObject);

            // Relayout maid icons
            var maidIconTransform = instance.transform.Find("ItemWindow/MaidIcon");
            maidIconTransform.localPosition = new Vector3(-170, 200);
            var maidUiGrid = maidIconTransform.gameObject.GetComponent<UIGrid>();
            maidUiGrid.arrangement = UIGrid.Arrangement.Vertical;
            maidUiGrid.pivot = UIWidget.Pivot.TopLeft;

            return instance;
        }

        public static GameObject CreateHalfUndressWidget(GameObject parent)
        {
            var obj = UIUtils.GetAtlas("AtlasCommon");
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
            var obj = UIUtils.GetAtlas("AtlasCommon");
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
            var instance = wf.Utility.CreatePrefab(parent, "SceneDance/Rhythm_Action/Prefab/UndressDance/MaidIcon", true);

            // setup new handler
            instance.AddComponent<MaidIcon>();
            
            var icon = instance.transform.Find("IconMask/Icon").gameObject;
            Assert.IsNotNull(icon, "Could not find IconMask/Icon");
            GameObject.Destroy(icon.GetComponent<UIWFTabButton>());

            return instance;
        }

        public static GameObject CreateItemIcon(GameObject parent)
        {
            GameObject instance = wf.Utility.CreatePrefab(parent, "SceneDance/Rhythm_Action/Prefab/UndressDance/ItemIcon", true);
            GameObject.Destroy(instance.GetComponent<Dance.UndressItem>());
            instance.AddComponent<UndressItemManager>();
            return instance;
        }
    }
}
