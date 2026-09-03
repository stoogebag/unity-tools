#if UNITY_2022_2_OR_NEWER
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace stoogebag.EditorTools
{
    [Overlay(typeof(EditorWindow), "stoogebag-game-speed", "Game Speed", true)]
    public class GameSpeedToolbar : ToolbarOverlay
    {
        const string PrefKey = "Chromata.GameSpeed";

        public GameSpeedToolbar() : base(
            GameSpeedButton0.Id,
            GameSpeedButton1.Id,
            GameSpeedButton2.Id,
            GameSpeedButton3.Id)
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        internal static float CurrentSpeed
        {
            get => EditorPrefs.GetFloat(PrefKey, 1f);
            set => EditorPrefs.SetFloat(PrefKey, value);
        }

        internal static void SetSpeed(float speed)
        {
            CurrentSpeed = speed;
            if (Application.isPlaying)
                Time.timeScale = speed;
            Refresh();
        }

        internal static void Refresh()
        {
            var current = CurrentSpeed;
            foreach (var b in GameSpeedButton.All)
                if (b != null)
                    b.text = b.BaseLabel + (Mathf.Approximately(b.Speed, current) ? "  ◀" : "");
        }

        static void OnPlayModeChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode)
                Time.timeScale = CurrentSpeed;
            Refresh();
        }

        public abstract class GameSpeedButton : EditorToolbarButton
        {
            public static readonly List<GameSpeedButton> All = new();
            public abstract float Speed { get; }
            public string BaseLabel { get; protected set; }
            protected GameSpeedButton() : base() => All.Add(this);
            protected GameSpeedButton(System.Action onClick) : base(onClick) => All.Add(this);
        }

        [EditorToolbarElement(Id, typeof(EditorWindow))]
        public class GameSpeedButton0 : GameSpeedButton
        {
            public const string Id = "chromata-gamespeed-0";
            public GameSpeedButton0() : base(() => SetSpeed(0.1f)) { BaseLabel = "0.1x"; text = BaseLabel; }
            public override float Speed => 0.1f;
        }
        [EditorToolbarElement(Id, typeof(EditorWindow))]
        public class GameSpeedButton1 : GameSpeedButton
        {
            public const string Id = "chromata-gamespeed-1";
            public GameSpeedButton1() : base(() => SetSpeed(0.5f)) { BaseLabel = "0.5x"; text = BaseLabel; }
            public override float Speed => 0.5f;
        }
        [EditorToolbarElement(Id, typeof(EditorWindow))]
        public class GameSpeedButton2 : GameSpeedButton
        {
            public const string Id = "chromata-gamespeed-2";
            public GameSpeedButton2() : base(() => SetSpeed(1f)) { BaseLabel = "1x"; text = BaseLabel; }
            public override float Speed => 1f;
        }
        [EditorToolbarElement(Id, typeof(EditorWindow))]
        public class GameSpeedButton3 : GameSpeedButton
        {
            public const string Id = "chromata-gamespeed-3";
            public GameSpeedButton3() : base(() => SetSpeed(2f)) { BaseLabel = "2x"; text = BaseLabel; }
            public override float Speed => 2f;
        }
    }
}
#endif
