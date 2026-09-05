#if UNITY_EDITOR
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace stoogebag.DIALOGUE.CustomTimelineTracks.Dialogue.Editor
{
    [CustomTimelineEditor(typeof(DialogueClip))]
    public class DialogueClipEditor : ClipEditor
    {
        private static readonly Color SpeakerColor = new Color(1f, 0.85f, 0.4f);
        private static readonly Color LineColor = Color.white;
        private const float Padding = 4f;
        private const float LineSpacing = 2f;

        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            base.DrawBackground(clip, region);

            if (clip.asset is not DialogueClip dialogueClip || dialogueClip.template == null)
                return;

            var behaviour = dialogueClip.template;
            var speaker = string.IsNullOrWhiteSpace(behaviour.speakerName) ? "" : behaviour.speakerName.Trim();
            var line = string.IsNullOrWhiteSpace(behaviour.dialogueLine) ? "(no text)" : behaviour.dialogueLine.Trim();

            var rect = region.position;
            rect.xMin += Padding;
            rect.xMax -= Padding;

            if (rect.width <= 0f)
                return;

            var speakerStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                normal = { textColor = SpeakerColor },
                clipping = TextClipping.Clip
            };

            var lineStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Normal,
                normal = { textColor = LineColor },
                clipping = TextClipping.Clip
            };

            var speakerHeight = string.IsNullOrEmpty(speaker) ? 0f : speakerStyle.CalcHeight(new GUIContent(speaker), rect.width);
            var lineHeight = lineStyle.CalcHeight(new GUIContent(line), rect.width);

            var totalHeight = speakerHeight + (string.IsNullOrEmpty(speaker) ? 0f : LineSpacing) + lineHeight;
            var startY = rect.y + (rect.height - totalHeight) * 0.5f;

            if (!string.IsNullOrEmpty(speaker))
            {
                var speakerRect = new Rect(rect.x, startY, rect.width, speakerHeight);
                GUI.Label(speakerRect, speaker, speakerStyle);
                startY += speakerHeight + LineSpacing;
            }

            var lineRect = new Rect(rect.x, startY, rect.width, lineHeight);
            GUI.Label(lineRect, line, lineStyle);
        }
    }
}
#endif
