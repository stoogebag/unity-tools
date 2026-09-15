using UnityEngine;

namespace stoogebag.Audio.Music
{
    public class MusicProfileTrigger : MonoBehaviour
    {
        [SerializeField] private MusicProfile profile;

        private void Start()
        {
            if (MusicManager.Instance == null)
            {
                Debug.LogError("[MusicProfileTrigger] No MusicManager in scene.", this);
                return;
            }

            MusicManager.Instance.SetProfile(profile);
        }
    }
}
