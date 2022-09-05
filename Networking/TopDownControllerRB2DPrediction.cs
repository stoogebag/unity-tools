#if FISHNET
using System;
using FishNet;
using FishNet.Object;
using FishNet.Object.Prediction;
using stoogebag;
using UnityEngine;

/*
* 
* See TransformPrediction.cs for more detailed notes.
* 
*/


public class TopDownControllerRB2DPrediction : NetworkBehaviour
{
    public struct InputData
    {
        public bool Run;
        public float Horizontal;
        public float Vertical;

        public Vector2 CursorPosition;
                                    
            
        public InputData(bool run, float horizontal, float vertical, Vector2 cursor)
        {
            Run = run;
            Horizontal = horizontal;
            Vertical = vertical;
            CursorPosition = cursor;
        }
    }
    public struct ReconcileData
    {
        public Vector3 Position;
        public float Rotation; //z rotation is the 2d rotation
        public Vector3 Velocity;
            
        public ReconcileData(Vector3 position, float rotation, Vector3 velocity)
        {
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
        }
    }
    [SerializeField]
    private float _runSpeed = 20f;
    [SerializeField]
    private float _walkSpeed = 15f;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
        InstanceFinder.TimeManager.OnPostTick += TimeManager_OnPostTick;
    }

    private void OnDestroy()
    {
        if (InstanceFinder.TimeManager != null)
        {
            InstanceFinder.TimeManager.OnTick -= TimeManager_OnTick;
            InstanceFinder.TimeManager.OnPostTick -= TimeManager_OnPostTick;
        }
    }

    private void Update()
    {
        if (base.IsOwner)
        {
        }
    }

    private void TimeManager_OnTick()
    {
        if (base.IsOwner)
        {
            Reconciliation(default, false);
            CheckInput(out InputData md);
            Move(md, false);
        }
        if (base.IsServer)
        {
            Move(default, true);
        }
    }


    private void TimeManager_OnPostTick()
    {
        if (base.IsServer)
        {
            ReconcileData rd = new ReconcileData(transform.position, transform.rotation.z, _rigidbody.velocity);
            Reconciliation(rd, true);
        }
    }

    private void CheckInput(out InputData md)
    {
        md = default;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        bool run = Input.GetKey(KeyCode.LeftShift);
        var cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);             
            
        //if (horizontal == 0f && vertical == 0f)
        //    return;

        md = new InputData(run, horizontal, vertical,cursor.ToVector2());
    }

    [Replicate]
    private void Move(InputData md, bool asServer, bool replaying = false)
    {
        //Add extra gravity for faster falls.
        var moveSpeed = md.Run ? _runSpeed : _walkSpeed;
            
        Vector2 forces = new Vector2(md.Horizontal, md.Vertical) * moveSpeed;
        _rigidbody.AddForce(forces);
            
        //rotate
        var diff = md.CursorPosition - transform.position.ToVector2();
        transform.rotation = Quaternion.Euler(0,0, ((float)Math.Atan2(diff.y, diff.x)).ToDegrees());

    }

    [Reconcile]
    private void Reconciliation(ReconcileData rd, bool asServer)
    {
        transform.position = rd.Position;
        transform.rotation = Quaternion.Euler(0f,0f, rd.Rotation);
        _rigidbody.velocity = rd.Velocity;
    }


}



#endif