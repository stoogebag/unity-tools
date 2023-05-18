using System.Threading.Tasks;
using UnityEngine;

public class StateManager<TState> where TState : State
{
    public TState Current { get; private set; }
        
    
    
    private async Task ChangeState(TState s)
    {
        if (Current == s) return;

        
        Debug.Log($"Exiting state {Current.Name}...");
        await Current.Exit();
        Debug.Log($"Exited state {Current.Name}...");
        
        Current = s;
        
        Debug.Log($"entering state {s.Name}...");
        //await Current.Enter();
        Debug.Log($"entered state {s.Name}...");
    }

    public async Task<bool> TryChangeState(TState s)
    {
        //todo: maybe sometime we can't exit the state or something...
        //for now, assume we can always do it

        await ChangeState(s);
        return true;
    }
}

public abstract class State
{
    private StateManager<State> _manager;

    // public static virtual State Create(StateManager manager)
    // {
    //     var state = new State();
    //     
    //     _manager = manager;
    //     return 
    // }
    
    public string Name;
    public abstract Task Enter(State previousState); //previous state might be relevant, for example entering the pause state we might return to previous afterwards. should that happen here? not sure but for now it does
    public abstract Task Exit();
}