using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;
using UnityEngine;

public class PostProcessFeedbackManager : MonoBehaviour
{
    private MMF_Player _mmPlayer;
    private MMF_Bloom_URP _bloom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mmPlayer = GetComponent<MMF_Player>();
        _bloom = _mmPlayer.GetFeedbackOfType<MMF_Bloom_URP>();
        Bind();
        Play();
    }

    // Update is called once per frame
    public void Bind()
    {
        _bloom.FeedbackDuration = 0.333f;
        
        
        
    }

    public void Play()
    {
        _mmPlayer.PlayFeedbacks();
    }
}
