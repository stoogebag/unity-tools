
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

public class AssetMenus
{
    
    [MenuItem("GameObject/stooge/create virtualCamera aligned with view")]
    static void CreateVirtualCamera()
    {
        var go = new GameObject("VCam");
        var vcam = go.AddComponent<CinemachineCamera>();

        var sv = SceneView.lastActiveSceneView;
        vcam.transform.position = sv.camera.transform.position;

        //todo: only do this if orthographic scene view or it will break
        if (sv.camera.orthographic)
        {
            vcam.Lens.OrthographicSize = sv.camera.orthographicSize;
        }

    }
}
