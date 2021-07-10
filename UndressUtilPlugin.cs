using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.VR;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;

namespace COM3D2.UndressUtil.Plugin
{
    public static class Version
    {
        public const string NUMBER = "1.0.0.0";

#if DEBUG
        public const string RELEASE_TYPE = "prerelease";
#else
        public const string RELEASE_TYPE = "release";
#endif

#if CRC_SUPPORT
        public const string VARIANT = "crc";
#else
        public const string VARIANT = "standard";
#endif
    }

    [BepInPlugin("org.bepinex.plugins.com3d2.undressutil", "UndressUtil", Version.NUMBER)]
    public class UndressUtilPlugin: BaseUnityPlugin
    {
        private enum Scene
        {
            /// <summary>メイド選択(夜伽、品評会の前など)</summary>
            SceneCharacterSelect = 1,

            /// <summary>品評会</summary>
            SceneCompetitiveShow = 2,

            /// <summary>昼夜メニュー、仕事結果</summary>
            SceneDaily = 3,

            /// <summary>ダンス1(ドキドキ Fallin' Love)</summary>
            SceneDance_DDFL = 4,

            /// <summary>メイドエディット</summary>
            SceneEdit = 5,

            /// <summary>メーカーロゴ</summary>
            SceneLogo = 6,

            /// <summary>メイド管理</summary>
            SceneMaidManagement = 7,

            /// <summary>ショップ</summary>
            SceneShop = 8,

            /// <summary>タイトル画面</summary>
            SceneTitle = 9,

            /// <summary>トロフィー閲覧</summary>
            SceneTrophy = 10,

            /// <summary>Chu-B Lip 夜伽</summary>
            SceneYotogi_ChuB = 10,

            /// <summary>？？？</summary>
            SceneTryInfo = 11,

            /// <summary>主人公エディット</summary>
            SceneUserEdit = 12,

            /// <summary>起動時警告画面</summary>
            SceneWarning = 13,

            /// <summary>夜伽</summary>
            SceneYotogi = 14,

            /// <summary>ADVパート(kgスクリプト処理)</summary>
            SceneADV = 15,

            /// <summary>日付画面</summary>
            SceneStartDaily = 16,

            /// <summary>タイトルに戻る</summary>
            SceneToTitle = 17,

            /// <summary>MVP</summary>
            SceneSingleEffect = 18,

            /// <summary>スタッフロール</summary>
            SceneStaffRoll = 19,

            /// <summary>ダンス2(Entrance To You)</summary>
            SceneDance_ETY = 20,

            /// <summary>ダンス3(Scarlet Leap)</summary>
            SceneDance_SL = 22,

            /// <summary>回想モード</summary>
            SceneRecollection = 24,

            /// <summary>撮影モード</summary>
            ScenePhotoMode = 27,

            /// <summary>Guest mode</summary>
            SceneGuestMode = 53,

            /// <sumary>Scout mode</sumary>
            SceneScoutMode = 114
        }

        private static int[] EnableScenes = {
            (int)Scene.SceneADV,
            (int)Scene.SceneRecollection,
            (int)Scene.SceneGuestMode,
            (int)Scene.SceneScoutMode,
        };

        public static UndressUtilPlugin Instance { get; private set; }

        public new UndressUtilConfig Config { get; private set; }

        internal new ManualLogSource Logger { get
            {
                return base.Logger;
            }
        }

        public void Awake()
        {
            if (UndressUtilPlugin.Instance != null)
            {
                throw new Exception("Already initialized");
            }

            GameObject.DontDestroyOnLoad(this);
            UndressUtilPlugin.Instance = this;
            this.Config = new UndressUtilConfig(base.Config);

            //NDebugShim.Init();
            //MaidShim.Init();
            //BaseKagManagerShim.Init();
            Log.LogInfo("Plugin initialized. Version {0}-{1} ({2})", Version.NUMBER, Version.VARIANT, Version.RELEASE_TYPE);
        }

        public void OnLevelWasLoaded(int level)
        {
            if (Config.disableSceneRestrictions.Value || EnableScenes.Contains(level))
            {
                Log.LogInfo("Setting up UndressWindow");
                SetupWindow();
            } else
            {
                Log.LogInfo("Current level [{0}] is not supported", level);
            }
        }

        public void SetupWindow()
        {
            GameObject uiroot = GameObject.Find("SystemUI Root");
            Assert.IsNotNull(uiroot, "Could not find SystemUI Root");
            Prefabs.CreateUndressWindow(uiroot);
        }
    }
}
