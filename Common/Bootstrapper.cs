using UnityEngine;

public static class Bootstrapper
{
    private const string MainPrefabPath = "Prefabs/Managers/Managers"; 
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        var obj = Resources.Load(MainPrefabPath);
        if (obj == null) return;
        Object.DontDestroyOnLoad(Object.Instantiate(obj));
    }
}