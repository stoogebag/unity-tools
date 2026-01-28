#if UNIRX

using System.Collections.Generic;
using System.Linq;
using stoogebag.Extensions;
using stoogebag.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridInputManager : Singleton<GridInputManager>
{

    [SerializeField] private Camera Camera;
    [SerializeField] InputActionAsset inputActionAsset;
    private List<MangEnt> Movers;

    private PuzzGrid Grid;
    
    void Awake()
    {
        AssignGrid();
        if(Camera == null) Camera = Camera.main;
    }

    private void OnEnable()
    {
        BindInputs();
    }

    private void BindInputs()
    {
        var moveAction = inputActionAsset.FindAction("Move");
        moveAction.Enable();

        moveAction.RepeatOnHold<Vector2>(rawVec => NearestCardinal(rawVec), 100, new []{500,250})
            .Where(v=> v != Vector2.zero) 
            .Subscribe(v =>
            {
                var dir = CameraDirectionRelativeToCam(Camera, GetDirection(v));
                HandleMove(dir);
            }).AddTo(this);
        
        //undo
        var undoAction = inputActionAsset.FindAction("Undo");
        undoAction.Enable();
        
        undoAction.RepeatOnHold( 100, new []{500,250})
            .Subscribe(v =>
            {
                Grid.RequestUndo();
            }).AddTo(this);

        //reset
        var resetAction = inputActionAsset.FindAction("Reset");
        resetAction.Enable();

        resetAction.OnPerformedAsObservable(100)
            .Subscribe(u =>
            {
                Grid.RequestReset();
            }).AddTo(this);

    }

    private Vector2 NearestCardinal(Vector2 rawVec)
    {
        var deadZone = 0.2f;
        if (rawVec.x > deadZone || rawVec.x < -deadZone || rawVec.y > deadZone || rawVec.y < -deadZone)
        {
            if (Mathf.Abs(rawVec.x) > Mathf.Abs(rawVec.y)) 
                return new Vector2(Mathf.Sign(rawVec.x), 0);
            else 
                return new Vector2(0,Mathf.Sign(rawVec.y));
        }
        else
            return Vector2.zero;
        
    }

    private void AssignGrid()
    {
        Grid = FindObjectOfType<PuzzGrid>();
        Movers=FindObjectsOfType<MangEnt>(false).ToList();
    }

    // private void HandleInput(PlayerAction a)
    // {
    //     if (Grid == null || !Grid.isActiveAndEnabled)
    //     {
    //         AssignGrid();
    //     }
    //     
    //     HandleMove(a);
    //     
    //     if (a == actions.Undo)
    //     {
    //         Grid.MoveQueue.AddAction(async () =>
    //         {
    //             await Grid.RequestUndo();
    //         });
    //     }
    //     else if (a == actions.Reset)
    //     {
    //         Grid.RequestReset();
    //     }
    //     else if (a == actions.NextLevel)
    //     {
    //         Grid.NextLevel();
    //     }
    //     else if (a == actions.PrevLevel)
    //     {
    //         Grid.PrevLevel();
    //     }
    //     else if (a == actions.Pause)
    //     {
    //         Grid.PauseUnpause();
    //     }
    //     else if (a == actions.Grow)
    //     {
    //         HandleGrow();
    //     }
    //     
    // }
    //
    private void HandleMove(Vector3 dir)
    {
        if(dir == Vector3.zero) return;
    
        Grid.MoveQueue.AddAction(async () =>
        {
            var multiplier = (Input.GetKey(KeyCode.LeftControl) ? 1 : 10) ;
            var sets = Movers.Select(t => t.GetWalkMove( dir*multiplier*t.gameObject.transform.localScale.x));
            var gp = new GridActionSetGroup(Grid) { ActionSets = sets.ToList() };
    
            await Grid.AddActionSetGroup(gp);
        });
    }

    private void HandleGrow()
    {
        Grid.MoveQueue.AddAction(async () =>
        {
      //      var action = new CompoundGrowAction(FindObjectOfType<ElevatorEnt>(), Vector3.up, 10, false, false);
        
         //   await Grid.AddActionSetGroup(GridActionSetGroup.GetSingle(Grid ,action));
        });
    }

   
    
    public Vector3 GetDirection(Vector2 value)
    {
        return new Vector3(value.x, 0, value.y);
    }

    //returns best nsew direction 
    public Vector3 CameraDirectionRelativeToCam(Camera cam, Vector3 dir)
    {
        //return dir;
        
        //todo: decide.
        var transformed = dir.RelativeTo(cam.transform);

        if(transformed == Vector3.zero) return Vector3.zero;
        //snap it to a standard basis vector
        if (Mathf.Abs(transformed.x) > Mathf.Abs(transformed.z))
        {
            return new Vector3(Mathf.Sign(transformed.x), 0, 0);
        }
        else {
            return new Vector3(0,0,Mathf.Sign(transformed.z));
        }

        return dir;
    }
    
}
#endif