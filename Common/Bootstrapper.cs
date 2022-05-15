using UnityEngine;

public static class Bootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        var obj = Resources.Load("Systems");
        if (obj == null) return;
        Object.DontDestroyOnLoad(Object.Instantiate(obj));
    }
}