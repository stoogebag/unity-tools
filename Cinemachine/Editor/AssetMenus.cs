#if UNITY_EDITOR

#if NEW_CINEMACHINE
using Unity.Cinemachine;
#else 
using Cinemachine;
#endif

using UnityEditor;
using UnityEngine;

public class AssetMenus
{
    
    [MenuItem("GameObject/stooge/create virtualCamera aligned with view")]
    static void CreateVirtualCamera()
    {
        var go = new GameObject("VCam");
        
#if NEW_CINEMACHINE
        var vcam = go.AddComponent<CinemachineCamera>();
        
        var sv = SceneView.lastActiveSceneView;
        vcam.transform.position = sv.camera.transform.position;

        //todo: only do this if orthographic scene view or it will break
        if (sv.camera.orthographic)
        {
            vcam.Lens.OrthographicSize = sv.camera.orthographicSize;
        }
        
#else 
        var vcam = go.AddComponent<CinemachineVirtualCamera>();
        
        var sv = SceneView.lastActiveSceneView;
        vcam.transform.position = sv.camera.transform.position;

        //todo: figure it out in case of old CM
        if (sv.camera.orthographic)
        {
            //vcam.Lens.OrthographicSize = sv.camera.orthographicSize;
        }
#endif


    }
}
#endif