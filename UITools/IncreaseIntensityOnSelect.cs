using DG.Tweening;
using stoogebag.Extensions;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(SelectableStateListener))]
public class IncreaseIntensityOnSelect : MonoBehaviour
{
    private SelectableStateListener ssl;

    [SerializeField]
    private float intensitySelected = 2f;
    
    [SerializeField]
    private float intensityNormal = 1f;

    [SerializeField] private string materialPropertyName = "_EmissionColor";

    [SerializeField] private bool relativeToIntensityOnAwake = true;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        ssl = GetComponent<SelectableStateListener>();

        myRenderer =  GetComponent<Renderer>();
        baseIntensity = relativeToIntensityOnAwake ? material.GetColor(materialPropertyName).Intensity() : 1f;
        
        baseColor = material.GetColor(materialPropertyName);

        ssl.CurrentState.Subscribe(state =>
        {

            if (state == SelectableStateListener.SelectionState.Normal)
            {
                SetSelected(false);
            }
            else
            {
                SetSelected(true);
            }
            
        });
    }

    private void SetSelected(bool value)
    {
        var targetIntensity =  (value ? intensitySelected : intensityNormal);
        var tweenTime = value ? 0.2f : 0.1f;
        _tween?.Kill();
        var targetColor = (baseColor * (targetIntensity ));
        
        _tween = DOTween.To(() => material.GetColor(materialPropertyName), x => material.SetColor(materialPropertyName, x), targetColor, tweenTime);

        _tween.Play();
    }

    private Tween _tween;
    private float baseIntensity;
    private Color baseColor;
    private Material material => myRenderer.material;
    
    Renderer myRenderer;
}
