#if FISHNET
using System;
using FishNet;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using stoogebag;
using stoogebag.Extensions;
using UnityEngine;

/*
* 
* See TransformPrediction.cs for more detailed notes.
* 
*/


public class TopDownControllerRB2DPrediction : NetworkBehaviour
{
    public struct InputData : IReplicateData 
    {
        public bool Run;
        public float Horizontal;
        public float Vertical;

        public Vector2 CursorPosition;
        private uint _tick;


        public InputData(bool run, float horizontal, float vertical, Vector2 cursor)
        {
            _tick = 0;
            Run = run;
            Horizontal = horizontal;
            Vertical = vertical;
            CursorPosition = cursor;
        }

        public uint GetTick()
        {
            return _tick;
        }

        public void SetTick(uint value)
        {
            _tick = value;
        }

        public void Dispose()
        {
        }
    }
    public struct ReconcileData : IReconcileData
    {
        public float AngularVelocity;
        public Vector3 Position;
        public float Rotation; //z rotation is the 2d rotation
        public Vector3 Velocity;
        private uint _tick;
            
        public ReconcileData(Vector3 position, float rotation, Vector3 velocity, float angularVelocity)
        {
            AngularVelocity = angularVelocity;
            _tick = 0;
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
        }

        public uint GetTick() => _tick;

        public void SetTick(uint value)
        {
            _tick = value;
        }

        public void Dispose()
        {
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
    }

    private void TimeManager_OnTick()
    {
        Move(BuildMoveData());
        
    }

    private InputData BuildMoveData()
    {
        if (base.IsOwner)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            bool run = Input.GetKey(KeyCode.LeftShift);
            var cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);             
            
            //if (horizontal == 0f && vertical == 0f)
            //    return;
            return  new InputData(run, horizontal, vertical,cursor.ToVector2());
        }
        else return default;
    }



    [ReplicateV2]
    private void Move(InputData md, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable)
    {
        var moveSpeed = md.Run ? _runSpeed : _walkSpeed;
            
        Vector2 forces = new Vector2(md.Horizontal, md.Vertical) * moveSpeed;
        _rigidbody.AddForce(forces);
            
        //rotate
        var diff = md.CursorPosition - transform.position.ToVector2();
        //transform.rotation = Quaternion.Euler(0,0, ((float)Math.Atan2(diff.y, diff.x)).ToDegrees());
    }
    
    private void TimeManager_OnPostTick()
    {
        if (IsServer)
        {
            ReconcileData rd = new ReconcileData(transform.position, transform.rotation.z, _rigidbody.velocity, _rigidbody.angularVelocity);
            Reconciliation(rd);
        }
    }


    [ReconcileV2]
    private void Reconciliation(ReconcileData rd, Channel channel = Channel.Unreliable)
    {
        transform.position = rd.Position;
        transform.rotation = Quaternion.Euler(0f,0f, rd.Rotation);
        //_rigidbody.velocity = rd.Velocity;
        _rigidbody.angularVelocity = rd.AngularVelocity;
    }


}



#endif