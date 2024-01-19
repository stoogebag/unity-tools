using UniRx;
using UnityEngine;

namespace stoogebag.Input
{
    public abstract class InputSchemeBase : MonoBehaviour
    {
        public abstract float GetHorizontal();
        public abstract float GetVertical();

        public abstract Vector3 GetLookTarget();

        public ReactiveProperty<bool> ActionButtonValue = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> ShootButtonValue = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> ItemButtonValue= new ReactiveProperty<bool>();
        public ReactiveProperty<bool> JumpButtonValue= new ReactiveProperty<bool>();

        public ReactiveProperty<bool> SprintButtonValue= new ReactiveProperty<bool>();
        public ReactiveProperty<bool> AbilityButtonValue= new ReactiveProperty<bool>();
        
        public ReactiveProperty<bool> BackButtonValue= new ReactiveProperty<bool>();

        public ReactiveProperty<bool> UpButtonValue= new ReactiveProperty<bool>();
        public ReactiveProperty<bool> DownButtonValue= new ReactiveProperty<bool>();
        public ReactiveProperty<bool> LeftButtonValue= new ReactiveProperty<bool>();
        public ReactiveProperty<bool> RightButtonValue= new ReactiveProperty<bool>();

        public ReactiveProperty<bool> SelectLeftButtonValue= new ReactiveProperty<bool>();
        public ReactiveProperty<bool> SelectRightButtonValue= new ReactiveProperty<bool>();

    
        /// <summary>
        /// this should be unique to the controller i think
        /// </summary>
        /// <returns></returns>
        public abstract string GetID();

    }

    public interface IHasInputScheme
    {
        public InputSchemeBase InputScheme { get; }
        void SetInput(InputSchemeBase input);
    }
}
