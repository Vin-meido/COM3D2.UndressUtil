﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.VR;
using BepInEx;
using BepInEx.Configuration;

namespace COM3D2.UndressUtil.Plugin
{
    [BepInPlugin("org.bepinex.plugins.com3d2.undressutil", "UndressUtil", "1.0.0.0")]
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

        private GameObjectSearcher searcher = new GameObjectSearcher();

        public new UndressUtilConfig Config { get; private set; }

        public void Awake()
        {
            if (UndressUtilPlugin.Instance != null)
            {
                throw new Exception("Already initialized");
            }

            GameObject.DontDestroyOnLoad(this);
            UndressUtilPlugin.Instance = this;
            this.Config = new UndressUtilConfig(base.Config);
        }

        public void OnLevelWasLoaded(int level)
        {
            if (EnableScenes.Contains(level))
            {
                SetupWindow();
            }
        }

        public void SetupWindow()
        {
            GameObject uiroot = GameObject.Find("SystemUI Root");
            Assert.IsNotNull(uiroot, "Could not find SystemUI Root");
            Prefabs.CreateUndressWindow(uiroot);
        }

        internal void LogInfo(string message, params object[] args)
        {
            //if (!enableDebug) return;
            Logger.LogInfo(string.Format(message, args));
        }

        internal void LogError(string message, params object[] args)
        {
            //if (!enableDebug) return;
            Logger.LogError(string.Format(message, args));
        }
    }
}