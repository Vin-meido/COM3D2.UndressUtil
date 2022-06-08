using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using BepInEx;
using BepInEx.Logging;

using COM3D2API;

namespace COM3D2.UndressUtil.Plugin
{
    public static class Version
    {
        public const string NUMBER = "1.3.0.6";

#if DEBUG
        public const string RELEASE_TYPE = "debug";
#else
        public const string RELEASE_TYPE = "release";
#endif

        public const string VARIANT = "standard";
    }

    [BepInPlugin("org.bepinex.plugins.com3d2.undressutil", "UndressUtil", Version.NUMBER)]
    [BepInDependency("deathweasel.com3d2.api", BepInDependency.DependencyFlags.HardDependency)]
    public class UndressUtilPlugin: BaseUnityPlugin
    {
        private enum SceneTypeEnum
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
            (int)SceneTypeEnum.SceneADV,
            (int)SceneTypeEnum.SceneRecollection,
            (int)SceneTypeEnum.SceneGuestMode,
            (int)SceneTypeEnum.SceneScoutMode,
        };

        public static UndressUtilPlugin Instance { get; private set; }

        public static UndressWindowManager Manager { get; private set; }

        public static bool IsOnOrPastTitleScreen { get; private set; } = false;

        public new UndressUtilConfig Config { get; private set; }

        internal new ManualLogSource Logger { get
            {
                return base.Logger;
            }
        }

        public bool IsAutoShow
        {
            get
            {
                if (GameMain.Instance.VRMode)
                {
                    return Config.autoShowInVr.Value;
                }
                else
                {
                    return Config.autoShowInNonVr.Value;
                }
            }
        }


        public bool IsAutoHide => Config.autoHide.Value;

        public bool IsInSupportedLevel
        {
            get
            {
                if (Config.autoShowInAllScenes.Value) return true;
                if (EnableScenes.Contains(currentLevel)) return true;
                if (IsInYotogiLevel && Config.autoShowInYotogi.Value) return true;

                return false;
            }
        }

        public bool IsInYotogiLevel
        {
            get
            {
                return currentLevel == (int)SceneTypeEnum.SceneYotogi || currentLevel == (int)SceneTypeEnum.SceneYotogi_ChuB;
            }
        }

        public bool IsInTitleLevel => currentLevel == (int)SceneTypeEnum.SceneTitle;

        public bool IsKeepYotogiDressState => Config.keepYotogiUndressState.Value;

        private int currentLevel;

        void Awake()
        {
            if (UndressUtilPlugin.Instance != null)
            {
                throw new Exception("Already initialized");
            }

            GameObject.DontDestroyOnLoad(this);
            UndressUtilPlugin.Instance = this;
            this.Config = new UndressUtilConfig(base.Config);
            Log.LogInfo("Plugin load complete!");
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            currentLevel = scene.buildIndex;
            if (IsInTitleLevel)
            {
                IsOnOrPastTitleScreen = true;
            }

            StopCoroutine(nameof(AutoShowStartCoroutine));
            if (IsAutoShow && IsOnOrPastTitleScreen && IsInSupportedLevel)
            {
                StartCoroutine(nameof(AutoShowStartCoroutine));
            }
        }

        void Start()
        {
            Log.LogInfo("Plugin startup...");

            SystemShortcutAPI.AddButton(
                "Undress utility", 
                SystemShortcutCallback,
                "Undress utility",
                GetIcon());

            SceneManager.sceneLoaded += this.OnSceneLoaded;
            StartCoroutine(KeyboardCheckCoroutine());

            Log.LogInfo("Plugin startup complete. Version {0}-{1} ({2})", Version.NUMBER, Version.VARIANT, Version.RELEASE_TYPE);
        }

        byte[] GetIcon()
        {
            var assembly = GetType().Assembly;
            using (var stream = assembly.GetManifestResourceStream("COM3D2.UndressUtil.Plugin.Icon.png"))
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
                return buffer;
            }
        }

        private UndressWindowManager EnsureManagerInitialized()
        {
            if(Manager == null)
            {
                Log.LogInfo("Initializing UndressWindowManager...");
                GameObject uiroot = GameObject.Find("SystemUI Root");
                Assert.IsNotNull(uiroot, "Could not find SystemUI Root");
                var obj = Prefabs.CreateUndressWindow(uiroot);
                Manager = obj.GetComponent<UndressWindowManager>();
                Log.LogInfo("UndressWindowManager initialization complete!");
            }

            return Manager;
        }

        private void SystemShortcutCallback()
        {
            StartCoroutine(SystemShortcutCallbackCoroutine());
        }

        private IEnumerator SystemShortcutCallbackCoroutine()
        {
            var manager = EnsureManagerInitialized();
            yield return null;
            manager.ToggleWindow();
        }

        private IEnumerator KeyboardCheckCoroutine()
        {
            while (true)
            {
                yield return null;

                var shortcut = UndressUtilPlugin.Instance.Config.showShortcut.Value;
                if (shortcut.IsDown())
                {
                    EnsureManagerInitialized();
                    yield return null;
                    Log.LogInfo("Toggling undress window window...");
                    Manager.ToggleWindow();
                }
            }
        }

        private IEnumerator AutoShowStartCoroutine()
        {
            yield return new WaitForSeconds(1);
            Log.LogInfo("AutoShow start");
            var manager = EnsureManagerInitialized();
            yield return null;
            manager.DelayedShowWindow();
            Log.LogInfo("Delayed show queued 1s");
        }
    }
}
