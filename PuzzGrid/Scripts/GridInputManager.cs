using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using InControl;
using stoogebag.Extensions;
using stoogebag.Utils;
using UnityEngine;

public class GridInputManager : Singleton<GridInputManager>
{
    GridPlayerActions actions;

    private List<MangEnt> Movers;

    private PuzzGrid Grid;
    
    void Awake()
    {
        HandleBindings();
             
        AssignGrid();
    }

    private void AssignGrid()
    {
        Grid = FindObjectOfType<PuzzGrid>();
        Movers=FindObjectsOfType<MangEnt>(false).ToList();
    }

    private void HandleBindings()
    {
        actions = new GridPlayerActions();

        actions.West.AddDefaultBinding(Key.A);
        //actions.West.AddDefaultBinding(Key.LeftArrow);
        actions.West.AddDefaultBinding(InputControlType.DPadLeft);
        actions.West.AddDefaultBinding(InputControlType.LeftStickLeft);


        actions.East.AddDefaultBinding(Key.D);
        //actions.East.AddDefaultBinding(Key.RightArrow);
        actions.East.AddDefaultBinding(InputControlType.DPadRight);
        actions.East.AddDefaultBinding(InputControlType.LeftStickRight);

        actions.North.AddDefaultBinding(Key.W);
        actions.North.AddDefaultBinding(InputControlType.DPadUp);
        actions.North.AddDefaultBinding(InputControlType.LeftStickUp);

        actions.South.AddDefaultBinding(Key.S);
        actions.South.AddDefaultBinding(InputControlType.DPadDown);
        actions.South.AddDefaultBinding(InputControlType.LeftStickDown);
        //actions.South.AddDefaultBinding(Key.DownArrow);

        actions.Grow.AddDefaultBinding(Key.UpArrow);
        actions.Grow.AddDefaultBinding(InputControlType.RightBumper);
        
        actions.Shrink.AddDefaultBinding(Key.DownArrow);
        actions.Shrink.AddDefaultBinding(InputControlType.LeftBumper);
        
        actions.Undo.AddDefaultBinding(Key.Z);
        actions.Undo.AddDefaultBinding(InputControlType.Action3);
        
        actions.Reset.AddDefaultBinding(Key.R);
        actions.Reset.AddDefaultBinding(InputControlType.Action4);

        actions.Pause.AddDefaultBinding(Key.Escape);
        actions.Pause.AddDefaultBinding(InputControlType.Start);
        
        actions.NextLevel.AddDefaultBinding(Key.L);
        actions.PrevLevel.AddDefaultBinding(Key.K);
    }

    private void Update()
    {
        //handle control!
        foreach (var a in actions.AllActions)
        {
            if (a.WasPressed)
            {
                HandleInput(a);
            }
        }
    }

    private void HandleInput(PlayerAction a)
    {
        if (Grid == null || !Grid.isActiveAndEnabled)
        {
            AssignGrid();
        }
        
        HandleMove(a);
        
        if (a == actions.Undo)
        {
            Grid.MoveQueue.AddAction(async () =>
            {
                await Grid.RequestUndo();
            });
        }
        else if (a == actions.Reset)
        {
            Grid.RequestReset();
        }
        else if (a == actions.NextLevel)
        {
            Grid.NextLevel();
        }
        else if (a == actions.PrevLevel)
        {
            Grid.PrevLevel();
        }
        else if (a == actions.Pause)
        {
            Grid.PauseUnpause();
        }
        else if (a == actions.Grow)
        {
            HandleGrow();
        }
        
    }
   
    private void HandleMove(PlayerAction a)
    {
        var dir = CameraDirectionRelativeToCam(Camera.main, GetDirection(a));
        if(dir == Vector3.zero) return;

        Grid.MoveQueue.AddAction(async () =>
        {
            var multiplier = (Input.GetKey(KeyCode.LeftControl) ? 1 : 10) ;
            var sets = Movers.Select(t => t.GetWalkMove( dir*multiplier*t.gameObject.transform.lossyScale.x));
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

   

    public Vector3 GetDirection(PlayerAction a)
    {
        switch (a.Name)
        {
            case "Move North":
                return new Vector3(0, 0, 1);
            case "Move West":
                return new Vector3(-1, 0, 0);
            case "Move South":
                return new Vector3(0, 0, -1);
            case "Move East":
                return new Vector3(1, 0, 0);
            default: 
                return Vector3.zero;
        }
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
