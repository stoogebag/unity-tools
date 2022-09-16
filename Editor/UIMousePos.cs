using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using stoogebag;
using UnityEditor;


[InitializeOnLoad]
public static class UIMousePos 
{

    static UIMousePos()
    {
        var screenRect = new Rect(10, Screen.height-30, 1000, 20);
        
        
        SceneView.onSceneGUIDelegate += view =>
        {
            //Debug.Log("ass");Ray ray;
            RaycastHit hit;

            var ray = Camera.current.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, SceneView.currentDrawingSceneView.camera.pixelHeight - Event.current.mousePosition.y));

            if (Physics.Raycast(ray, out hit))
            {
                var x1 = Input.mousePosition.x;
                var y1 = Input.mousePosition.y;
                var z1 = Input.mousePosition.z;
                var x2 = hit.point.x;
                var y2 = 0;
                var z2 = hit.point.z;
                
                //Debug.Log(z2);
                
                var s= $"({x2}, {y2}, {z2}), {hit.collider.gameObject.name}";
                
                Handles.BeginGUI();
                GUI.Label(screenRect, s);
                Handles.EndGUI();
            }
            
        };
    }

}
