using DG.Tweening;
using UniRx;
using UnityEngine;

namespace stoogebag.Audio.Music
{
    public class PulseOnBeat : MonoBehaviour
    {

        [SerializeField] private float beatAnticipate = 0.2f;

        [SerializeField] private float beatNumber;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            BeatManager.Instance.ActiveProvider.OnBeatAnticipated(beatNumber,beatAnticipate).Subscribe(async b =>
            {
                await gameObject.transform.DOScale(Vector3.one*1.1f, beatAnticipate).SetEase(Ease.InOutCubic).AsyncWaitForCompletion();
                gameObject.transform.localScale = Vector3.one * 1.2f;
                await gameObject.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutCubic).AsyncWaitForCompletion();

            }).AddTo(this);
        }

    }
}
