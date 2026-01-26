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

    [SerializeField] InputActionAsset inputActionAsset;
    private List<MangEnt> Movers;

    private PuzzGrid Grid;
    
    void Awake()
    {
        AssignGrid();
    }

    private void OnEnable()
    {
        var moveAction = inputActionAsset.FindAction("Move");
        print(moveAction);
        moveAction.Enable();

        var hold = moveAction.started;
        moveAction.OnPerformedAsObservable(1).Subscribe(e =>
        {
            var action = e.action;
            var dir = CameraDirectionRelativeToCam(Camera.main, GetDirection(action));
            HandleMove(dir);
        }).AddTo(this);
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

   
    
    public Vector3 GetDirection(InputAction a)
    {
        var value = a.ReadValue<Vector2>();
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