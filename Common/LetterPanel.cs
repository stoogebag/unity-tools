using System;
using stoogebag_MonuMental.stoogebag.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace stoogebag_MonuMental.stoogebag.Common
{
    public class LetterPanel : MonoBehaviour
    {

        public static char[] AlphabetArray = "ABCDEFGHIJKLMNOPQRSTUVWXYZ_.@!".ToCharArray();

        public int index;
        public char letter => AlphabetArray[index];

        public Button back;
        public Button forward;
        public TextMeshProUGUI text;
    
    
        event Action OnRefresh;
        public IObservable<Unit> RefreshObservable => Observable.FromEvent(h => OnRefresh += h, h => OnRefresh -= h);
    
    
        void Start()
        {
            back.OnClickAsObservable().Subscribe(t =>
            {
                index = (index - 1 + AlphabetArray.Length) % AlphabetArray.Length;
                Refresh();
            });
            forward.OnClickAsObservable().Subscribe(t =>
            {
                index = (index + 1) % AlphabetArray.Length;
            
                Refresh();
            });
        }

        private void Refresh()
        {
            text.text = letter.ToString();
            OnRefresh?.Invoke();
        }

        public void SetLetter(char c)
        {
            index = AlphabetArray.IndexOfFirst(t => t == c);
            if (index == -1) index = 0;
        
            Refresh();
        }
    
        public void Bind(char selected)
        {
            //textLabel.text = label;
            //OptionsArray = options;
            //IsAvailableFunc = availabilityFunc;
            SetLetter(Char.ToUpper(selected)); 
        }
    }
}
