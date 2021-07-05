using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace COM3D2.UndressUtil.Plugin
{
    public class PluginWindowManager: MonoBehaviour
    {
        public UIAtlas CommonAtlas { get; private set; }


        public static PluginWindowManager Create(string name, GameObject parent)
        {
            var ob = new GameObject(name);
            ob.SetActive(false);
            ob.layer = parent.layer;
            ob.transform.SetParent(parent.transform, false);
            return ob.AddComponent<PluginWindowManager>();
        }

        public void Awake()
        {
            GameObjectSearcher searcher = new GameObjectSearcher();
            GameObject ob = searcher.Find("AtlasCommon");
            this.CommonAtlas = ob.GetComponent<UIAtlas>();

            GameObject panelOb = new GameObject("parent");
            panelOb.layer = this.gameObject.layer;
            panelOb.transform.SetParent(this.gameObject.transform, false);
            UIPanel panel = panelOb.AddComponent<UIPanel>();
            var collider = panelOb.AddComponent<BoxCollider>();
            collider.size = new Vector3(100, 100, 1);
            panelOb.AddComponent<UIEventTrigger>();
            var dragger = panelOb.AddComponent<PhotoWindowDragMove>();
            dragger.WindowTransform = this.gameObject.transform;


            NGUITools.AddSprite(panel.gameObject, this.CommonAtlas, "cm3d2_common_plate_black");
        }

        public void Show()
        {
            this.gameObject.SetActive(true);

            var nob = GameObject.Find("COM3D2.PluginDrawer.MenuItemTarget");
            if (nob != null)
            {
                nob.BroadcastMessage("MenuItem", new Dictionary<string, object>()
                {
                    { "name", "Test name" },
                    { "action", new Action(this.Show) }
                });
            }

            var callback = new Action(this.Show);
            var t = new EventDelegate.Callback(callback);
            EventDelegate.Add(null, t);
        }
    }
}
