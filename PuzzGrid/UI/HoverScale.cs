using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,ISelectHandler, IDeselectHandler
{
    [SerializeField]
    Vector3 scale = Vector3.one * 1.1f;

    [SerializeField] private float transitionTime = 0.2f;

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.transform.DOScale(scale, transitionTime);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.transform.DOScale(Vector3.one, transitionTime);
    }

    public void OnSelect(BaseEventData eventData)
    {
        gameObject.transform.DOScale(scale, transitionTime);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        gameObject.transform.DOScale(Vector3.one, transitionTime);
    }

    public Tween tween;
}
