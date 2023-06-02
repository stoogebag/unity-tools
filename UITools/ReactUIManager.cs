#if REACT_UNITY

using ReactUnity.UGUI;
using Sirenix.OdinInspector;
using UnityEngine;

public class ReactUIManager : MonoBehaviour
{
    [Button]
    void BindGlobals()
    {
    }

    public string Route = "error";
    public string ReferenceName = "error";

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        
        GetComponent<ReactRendererUGUI>().Globals["route"] = "/"+Route;
        GetComponent<ReactRendererUGUI>().Globals[ReferenceName] = this;
    }
    
}

#endif