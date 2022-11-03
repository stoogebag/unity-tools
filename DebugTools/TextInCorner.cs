using UnityEngine;

namespace stoogebag_MonuMental.stoogebag.DebugTools
{
    public abstract class TextInCorner : MonoBehaviour
    {
        private enum Corner
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }
        /// <summary>
        /// Which corner to display in.
        /// </summary>
        [Tooltip("Which corner to display ping in.")]
        [SerializeField]
        private Corner _placement = Corner.TopRight;

        #region Private.
        /// <summary>
        /// Style for drawn text.
        /// </summary>
        private GUIStyle _style = new GUIStyle();
        #endregion

        private void OnGUI()
        {
            _style.normal.textColor = Color.white;
            _style.fontSize = 15;
            float width = 85f;
            float height = 15f;
            float edge = 10f;

            float horizontal;
            float vertical;

            if (_placement == Corner.TopLeft)
            {
                horizontal = 10f;
                vertical = 10f;
            }
            else if (_placement == Corner.TopRight)
            {
                horizontal = Screen.width - width - edge;
                vertical = 10f;
            }
            else if (_placement == Corner.BottomLeft)
            {
                horizontal = 10f;
                vertical = Screen.height - height - edge;
            }
            else
            {
                horizontal = Screen.width - width - edge;
                vertical = Screen.height - height - edge;
            }
            
            var text = GetText();
            GUI.Label(new Rect(horizontal, vertical, width, height), text, _style);
        }

        public abstract string GetText();

    }
}