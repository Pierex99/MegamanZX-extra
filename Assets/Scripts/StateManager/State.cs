
public abstract class State<T>
{
    protected FiniteStateMachine<T> mFsm;
    protected T mController;
    public State(T controller, FiniteStateMachine<T> fsm)
    {
        mFsm = fsm;
        mController = controller;
    }
    public virtual void OnStart() {}
    public virtual void OnStop() {}
    public virtual void OnPhysicsUpdate() {}
    public virtual void OnLogicUpdate() {}
    public virtual void HandleInput() {}
}
