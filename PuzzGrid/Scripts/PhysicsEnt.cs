using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using stoogebag.Extensions;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

[SelectionBase]
public class PhysicsEnt : MonoBehaviour
{
    public bool IsClone = false;

    private CompositeDisposable _disp = new CompositeDisposable();
    public bool Moved;

    public PhysicsEnt Parent;


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent?.TryGetComponent<Portal>(out var portal) == true)
        {
            EnteredPortalVicinity(portal);
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.parent?.TryGetComponent<Portal>(out var portal) == true)
        {
            EnteredPortalVicinity(portal);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent?.TryGetComponent<Portal>(out var portal) == true)
        {
            ExitedPortalVicinity(portal);
        }
    }


    private void EnteredPortalVicinity(Portal portal)
    {
        if(IsClone) return; //clones don't clone
        if(portalClones.ContainsKey(portal)) return; //already exists
        
        var clone = new PortalClone();
        clone.NearbyPortal = portal;
        clone.Original = this;
        clone.OtherPortal = portal.OtherPortal;
        
        var cloneEnt = Instantiate(gameObject, transform.position, transform.rotation);
        clone.Clone = cloneEnt.GetComponent<PhysicsEnt>();
        clone.Clone.IsClone = true;
        clone.Clone.Moved = false;
        clone.Clone.Parent = this;
        portalClones.Add(portal, clone);
        clone.Bind();


    }
    private void ExitedPortalVicinity(Portal portal)
    {
        if (portalClones.TryGetValue(portal, out var clone))
        {
            clone.Dispose();
            portalClones.Remove(portal);
        }
    }

    public void CheckPortals()
    {
        foreach (var (portal, clone) in portalClones)
        {
            var myPos = portal.transform.InverseTransformPoint(transform.position);
//            Debug.Log(myPos);
            //return;
            
            //bc: be careful that your entity's pivot is where you think it is!
            if (myPos.z > 0.01f)
            {
                //swop.
                var clonePos = clone.Clone.transform.position;
                var clonerot = clone.Clone.transform.rotation;
                
                transform.SetPositionAndRotation(clonePos, clonerot);
                transform.localScale = clone.Clone.transform.localScale;
                return;
            }
        }
    }
    
    Dictionary<Portal, PortalClone> portalClones = new Dictionary<Portal, PortalClone>();
}
public class PortalClone
{
    public Portal NearbyPortal; //this is the one we are near. 
    public Portal OtherPortal; //this is the partner of the one we are near.
    public PhysicsEnt Original;
    public PhysicsEnt Clone;

    CompositeDisposable _disposables = new CompositeDisposable();

    public TransformInfo _lastClonePos;
    public TransformInfo _lastOriginalPos;
    
    
    public static readonly Quaternion halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    public void Bind()
    {
        //Clone.GetComponentInChildren<MeshRenderer>().enabled = false; //todo: figure out what to do here. might need a custom shador
        
        _lastClonePos = new TransformInfo(Clone.transform);
        
        Original.gameObject.LateUpdateAsObservable().Subscribe(_ =>
        {

            var cloneMoved = !_lastClonePos.EqualsTransform(Clone.transform);
            
            var master = cloneMoved ? Clone : Original;
            var nonMaster = cloneMoved ? Original : Clone;
            
            var inPortal = cloneMoved ? OtherPortal : NearbyPortal;
            var outPortal = cloneMoved ? NearbyPortal : OtherPortal;
            
            var inTransform = inPortal.transform;
            var outTransform = outPortal.transform;

            // Update position of clone.
            Vector3 relativePos = inTransform.InverseTransformPoint(master.transform.position);
            relativePos = halfTurn * relativePos;
            nonMaster.transform.position = outTransform.TransformPoint(relativePos);
            
            // Update rotation of clone.
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * master.transform.rotation;
            relativeRot = halfTurn * relativeRot;
            nonMaster.transform.rotation = outTransform.rotation * relativeRot;
            
            //update scale of clone to match the portals' relative scales
            nonMaster.transform.localScale = master.transform.localScale * (outTransform.lossyScale.x/inTransform.lossyScale.x);
            Clone.Moved = false;
            _lastClonePos = new TransformInfo(Clone.transform);

        }).AddTo(_disposables);
        
        
    }
    
    public void Dispose()
    {
        _disposables.Dispose();
        GameObject.Destroy(Clone.gameObject);
    }

}