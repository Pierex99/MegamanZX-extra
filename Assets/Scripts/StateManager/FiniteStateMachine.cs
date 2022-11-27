public class FiniteStateMachine<T>
{
    public State<T> CurrentState {get; private set; }

    public void Start(State<T> initialState)
    {
        CurrentState = initialState;
        CurrentState.OnStart();
    }

    public void ChangeState(State<T> newState)
    {
        CurrentState.OnStop();
        CurrentState = newState;
        CurrentState.OnStart();
    }
}
