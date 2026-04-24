using MoreMountains.Feedbacks;
using UnityEngine;

public class MMFeedbacksPlayOnStart : MonoBehaviour
{
    private MMF_Player _mmPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mmPlayer = GetComponent<MMF_Player>();
        Play();
    }

    
    public void Play()
    {
        _mmPlayer.PlayFeedbacks();
    }
}
