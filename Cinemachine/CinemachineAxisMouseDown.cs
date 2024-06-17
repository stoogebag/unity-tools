
#if CINEMACHINE
using System.Collections;
using System.Collections.Generic;

#if UNITY6
using Unity.Cinemachine;
#else 
using Cinemachine;
#endif
using UnityEngine;
 
public class CinemachineAxisMouseDown : MonoBehaviour
{

    public enum MouseButtonType
    {
        Left,
        Right, 
        Both
    }
    
    public MouseButtonType MouseButton;

    bool IsMouseButtonDown()
    {
        if(MouseButton == MouseButtonType.Left && Input.GetMouseButton(0))
        {
            return true;
        }
        if(MouseButton == MouseButtonType.Right && Input.GetMouseButton(1))
        {
            return true;
        }
        if(MouseButton == MouseButtonType.Both && (Input.GetMouseButton(0) || Input.GetMouseButton(1)))
        {
            return true;
        }

        return false;
    }
    
    void Start(){
        CinemachineCore.GetInputAxis = GetAxisCustom;
    }
    public float GetAxisCustom(string axisName){
        if(axisName == "Mouse X"){
            if (IsMouseButtonDown()){
                return UnityEngine.Input.GetAxis("Mouse X");
            } else{
                return 0;
            }
        }
        else if (axisName == "Mouse Y"){
            if (IsMouseButtonDown()){
                return UnityEngine.Input.GetAxis("Mouse Y");
            } else{
                return 0;
            }
        }
        return UnityEngine.Input.GetAxis(axisName);
    }
}
#endif