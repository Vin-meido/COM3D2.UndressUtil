using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace COM3D2.UndressUtil.Plugin.UIHelper
{
    public class Button: MonoBehaviour
    {
        UILabel uiLabel;
        UISprite uiSprite;

        public Vector2 size { get
            {
                return new Vector2(uiSprite.width, uiSprite.height);
            }
            set
            {
                uiSprite.width = uiLabel.width = ((int)value.x);
                uiSprite.height = uiLabel.height = ((int)value.y);
                uiSprite.ResizeCollider();
            }
        }

        public string label
        {
            get
            {
                return uiLabel.text;
            }
            set
            {
                uiLabel.text = value;
            }
        }

        public Vector2 position
        {
            get
            {
                return new Vector2(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y);
            }
            set
            {
                gameObject.transform.localPosition = new Vector3(value.x, value.y, gameObject.transform.localPosition.z);
            }
        }

        public void Awake() {
            uiSprite = gameObject.GetComponent<UISprite>();
            NGUITools.AddWidgetCollider(gameObject);
            gameObject.AddComponent<UIButton>();

            uiLabel = NGUITools.AddWidget<UILabel>(gameObject);
            uiLabel.trueTypeFont = UIUtils.GetFont("NotoSansCJKjp-DemiLight");
            uiLabel.color = Color.black;
            uiLabel.text = "label";
        }

        public static Button Add(GameObject parent, UIAtlas atlas, string spriteName)
        {
            var sprite = NGUITools.AddSprite(parent, atlas, spriteName);
            var go = sprite.gameObject;
            return go.AddComponent<Button>();
        }
    }


    public class Panel: MonoBehaviour
    {
        Window window;



        public static Panel Create(Window parent)
        {
            return Create(parent.gameObject);
        }

        public static Panel Create(Panel parent)
        {
            return Create(parent.gameObject);
        }

        private static Panel Create(GameObject parent)
        {
            var obj = NGUITools.AddChild(parent);
            return obj.AddComponent<Panel>();
        }
    }


    public class Window: MonoBehaviour
    {
        public Font DefaultFont { get; private set; }
        public UIAtlas AtlasCommon { get; private set; }

        UIPanel panel;

        public void Init()
        {
            AtlasCommon = UIUtils.GetAtlas("AtlasCommon");
            DefaultFont = UIUtils.GetFont("NotoSansCJKjp - DemiLight");
            panel = gameObject.AddComponent<UIPanel>();
            panel.clipping = UIDrawCall.Clipping.ConstrainButDontClip;
        }


        public static Window Create(UIRoot root)
        {
            var obj = NGUITools.AddChild(root.gameObject);
            var window = obj.AddComponent<Window>();
            return window;
        }
    }
}
