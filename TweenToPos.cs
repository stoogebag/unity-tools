using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using stoogebag.Extensions;
using UnityEngine;

public class TweenToPos : MonoBehaviour
{
    public bool Rotate = true;
    public float Duration = 1f;

    public async UniTask MoveToPosition(Transform mover)
    {
        var moveTask =  mover.DOMove(transform.position, Duration).ToUniTask();
        var rotateTask = mover.DORotateQuaternion(transform.rotation, Duration).ToUniTask();
        
        if(Rotate) await UniTask.WhenAll(moveTask, rotateTask);
        else await moveTask;
        
    }
    
}
