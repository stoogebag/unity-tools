using stoogebag_MonuMental.stoogebag.Extensions;
using TMPro;
using UniRx;
using UnityEngine.UI;

namespace stoogebag_MonuMental.stoogebag.UITools.Windows
{
    public class SimpleTextWindow : TemporaryWindow<string, object>
    {
        private string _text;

        private TextMeshProUGUI text;

        private Button okButton;
        private Button cancelButton;

        private void Awake()
        {
            okButton = this.GetChild<Button>("ok");
            okButton?.OnClickAsObservable().Subscribe(t => TryProceed());
        
            cancelButton = this.GetChild<Button>("cancel");
            cancelButton?.OnClickAsObservable().Subscribe(t => TryCancel());

        }

        protected override object GetModel()
        {
            return null;
        }

        protected override void Bind(string messageText, object model = null)
        {
            text = GetComponentInChildren<TextMeshProUGUI>(true);

            _text = messageText;
            text.text = _text;
        }
    }
}