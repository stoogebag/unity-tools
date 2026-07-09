using UniRx;
using UnityEngine;

public class TransformChangedTracker : MonoBehaviour
{
    private void Awake()
    {
        gameObject.ObserveEveryValueChanged(t => t.transform)
            .Subscribe(t =>
            {
                Debug.Log("my transform was changed!", this);
            }).AddTo(this);
    }
}