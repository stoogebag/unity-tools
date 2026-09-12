using DG.Tweening;
using stoogebag.Extensions;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(SelectableStateListener))]
public class ScaleOnSelect : MonoBehaviour
{
    [SerializeField] private Vector3 ScaleFactor = new Vector3(1.1f, 1.1f, 1.1f);

    private ReactiveProperty<bool> _scale = new();

    private Tween _tween;

    // Start is called before the first frame update
    void Start()
    {
        //always uses the original localScale. todo: allow update of this?
        var scale = transform.localScale;
        
        var ssl = GetComponent<SelectableStateListener>();
        ssl.CurrentState.Subscribe(state =>{ _scale.Value = state == SelectableStateListener.SelectionState.Selected || state== SelectableStateListener.SelectionState.Highlighted; }).AddTo(this);
        
        _scale.Subscribe(t =>
        {
            if (t == null) return; //why is this here lool
            
            _tween?.Kill();
            if (t)
            {
                _tween = transform.DOScale(scale.MultiplyPointwise(ScaleFactor), .2f).SetEase(Ease.OutBack)
                    // .OnStart(() => Debug.Log($"{name} TWEEN START"))
                    // .OnUpdate(() => Debug.Log($"{name} TWEEN UPDATE {transform.localScale}"))
                    // .OnComplete(() => Debug.Log($"{name} TWEEN COMPLETE"))
                    .Play();
                // print(_tween.IsPlaying());
            }
            else
            {
                _tween = transform.DOScale(scale, .1f).SetEase(Ease.OutBack).Play();
            }
        });
    }
    

}
