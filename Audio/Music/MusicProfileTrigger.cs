using System.Collections;
using FMODUnity;
using UnityEngine;

namespace stoogebag.Audio.Music
{
    public class MusicProfileTrigger : MonoBehaviour
    {
        [SerializeField] private MusicProfile profile;
        [SerializeField] private string[] banks;
        [SerializeField] private string gameScene;

        private IEnumerator Start()
        {

            if (MusicManager.Instance == null)
            {
                Debug.LogError("[MusicProfileTrigger] No MusicManager in scene.", this);
                yield break;
            }

            foreach (var bank in banks)
            {
                RuntimeManager.LoadBank(bank, true);
            }

            while (!RuntimeManager.HaveAllBanksLoaded)
                yield return null;

            while (RuntimeManager.AnySampleDataLoading())
                yield return null;

            MusicManager.Instance.SetProfile(profile);
        }
    }
}
