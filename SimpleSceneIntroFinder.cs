using System.Linq;
using stoogebag.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleSceneIntroFinder : MonoBehaviour, ISceneIntroProvider
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public ISceneIntro GetSceneIntro(string sceneName)
    {
        //ew
        var intro = SceneManager.GetSceneByName(sceneName)
            .GetRootGameObjects()
            .Select(t =>
                t.gameObject.GetComponentsWithInterface<ISceneIntro>().FirstOrDefault())
            .WhereNotNull()
            .FirstOrDefault();
        
        return intro;
    }
}

public interface ISceneIntroProvider
{
    public ISceneIntro GetSceneIntro(string sceneName); 
}
public interface ISceneIntro
{
    
}