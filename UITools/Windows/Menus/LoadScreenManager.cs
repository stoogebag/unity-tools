using Cysharp.Threading.Tasks;
using UnityEngine;
using stoogebag.UITools.Windows;

public class LoadScreenManager : MonoBehaviour
{
    public static LoadScreenManager Instance { get; private set; }

    [SerializeField] private Window loadingWindow;

    [SerializeField] private float minimumTimeInSeconds = 1f; //this is to prevent an unsightly 'flash' of a load screen for a short-running task. 

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async UniTask Run(UniTask task)
    {
        await loadingWindow.Activate();
        await task;
        await loadingWindow.Deactivate();
    }
}

public static class UniTaskLoadScreenExtensions
{
    public static UniTask AwaitWithLoadScreen(this UniTask task)
    {
        return LoadScreenManager.Instance.Run(task);
    }
}
