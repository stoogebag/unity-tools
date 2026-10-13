using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace stoogebag.Phasing
{
    public class TimelinePhase : EntityPhase
    {
        public override async UniTask Enter()
        {
            gameObject.SetActive(true);

            var director = GetComponent<PlayableDirector>();
            if (director == null)
            {
                Exit();
                return;
            }

            if (!director.gameObject.activeSelf) director.gameObject.SetActive(true);
            director.Play();
            await UniTask.WaitWhile(() => director.state == PlayState.Playing);
            Exit();
        }
    }
}
