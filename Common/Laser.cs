using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
// using DG.Tweening;
// using Sirenix.OdinInspector;
// using Sirenix.OdinInspector.Editor;
using UnityEngine;

public class Laser : MonoBehaviour, ICollides
{

    private float oldDistance;
    [SerializeField] private float transitionTime = 0.1f;
    [SerializeField] private float rayLength = 100f;


    [SerializeField] private GameObject end;
    
    // Update is called once per frame
    void Update()
    {
        
        //if(TimeExtensions.NthFrame(5))
        {
            UpdateLaser();
        };
    }

    [Button]
    private void UpdateLaser()
    {
        //raycast to determine length
        var ray = new Ray(transform.position, transform.forward);
        oldDistance = transform.localScale.z;
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        //cast the ray
        if (Physics.Raycast(ray, out var hit))
        {
            if (hit.distance < oldDistance)
            {
                transform.localScale = new Vector3(1, 1, hit.distance);
                end.transform.localScale = new Vector3(1,  1,1 / hit.distance);
            }
            else if (hit.distance > oldDistance + 1)
            {
                //if the laser's getting longer, ease it out. we use a raw duration not a speed cos its so fast anyway who cares.
                //this might have serious issues creating junk each frame. 
                //the +1 in the condition above should handle it well enough cos its probably not a big deal
                
                transform.DOScaleZ(hit.distance, transitionTime);

                end.transform.DOScaleZ(1 / hit.distance, transitionTime);

                //transform.localScale = new Vector3(1, 1, hit.distance);
                //end.transform.localScale = new Vector3(1,  1 / hit.distance, 1);
            }


            if (hit.collider.gameObject.TryGetComponent<ILaserReceiver>(out var receiver))
                receiver.ReceiveLaser();
        }
        else
        {
            //if it doesn't hit anything, set the length to 500
            transform.localScale = new Vector3(1, 1, rayLength);
            end.transform.localScale = new Vector3(1,  1f / rayLength,1);
        }
    }
}

public interface ILaserReceiver
{
    void ReceiveLaser();
}

public interface ICollides
{
    
}