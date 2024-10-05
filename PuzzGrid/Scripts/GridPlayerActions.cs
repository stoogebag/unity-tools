#if INCONTROL_EXISTS

using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;

public class GridPlayerActions : PlayerActionSet
{
    //movement
    public PlayerAction North;
    public PlayerAction South;
    public PlayerAction East;
    public PlayerAction West;

    public PlayerTwoAxisAction Move;

    //actions
    public PlayerAction Grow;
    public PlayerAction Shrink;

    //undo
    public PlayerAction Undo;
    public PlayerAction Reset;
    
    //app
    public PlayerAction Pause;

    //debug
    public PlayerAction NextLevel;
    public PlayerAction PrevLevel;

    public List<PlayerAction> AllActions = new List<PlayerAction>();
    
    public GridPlayerActions()
    {
        North = CreatePlayerAction( "Move North" );
        South = CreatePlayerAction( "Move South" );
        East = CreatePlayerAction( "Move East" );
        West = CreatePlayerAction( "Move West" );
        Move = CreateTwoAxisPlayerAction(West, East, South, North);
        
        Grow = CreatePlayerAction( "Grow" );
        Shrink = CreatePlayerAction( "Shrink" );
        
        Undo = CreatePlayerAction( "Undo" );
        Reset = CreatePlayerAction( "Reset" );

        Pause = CreatePlayerAction( "Pause" );
        
        NextLevel = CreatePlayerAction( "NextLevel" );
        PrevLevel = CreatePlayerAction( "PrevLevel" );

        AllActions = new List<PlayerAction>()
        {
            North,
            South,
            East,
            West,
            Grow,
            Shrink,
            Undo,
            Reset,
            Pause,
            NextLevel,
            PrevLevel,
        };

    }
    
    
}
#endif