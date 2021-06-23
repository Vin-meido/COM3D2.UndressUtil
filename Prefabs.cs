using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.UndressUtil.Plugin
{
    class Prefabs
    {
        public static GameObject CreateUndressWindow(GameObject parent)
        {
            var ob = Resources.Load<GameObject>("SceneDance/Rhythm_Action/Prefab/RhythmAction_Mgr");
            var prefab = ob.transform.Find("DanceUndress").gameObject;

            var instance = UnityEngine.Object.Instantiate<GameObject>(prefab, parent.transform, false);

            // Remove default handlers
            GameObject.Destroy(instance.GetComponent<UndressDance_Mgr>());
            GameObject.Destroy(instance.GetComponentInChildren<UIWFTabPanel>());

            // Setup maid status tracking
            instance.AddComponent<MaidTracker>();

            // Setup new handler
            instance.AddComponent<UndressWindowManager>();

            // Set default name
            instance.name = "UndressWindow";
            return instance;
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
            instance.AddComponent<UndressItem>();
            return instance;
        }
    }
}
