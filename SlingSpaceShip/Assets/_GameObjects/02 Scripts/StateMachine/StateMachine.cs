public class StateMachine<TContext>
{
    private IState<TContext> currentState;
    private readonly TContext context;

    public StateMachine(TContext context)
    {
        this.context = context;
    }
    
    public void SetState(IState<TContext> newState)
    {
        if (currentState == newState)
        {
            return;
        }
        
        currentState?.Exit(context);
        
        currentState = newState;
        
        currentState?.Enter(context);
    }

    public void UpdateState(float deltaTime)
    {
        currentState?.UpdateState(context, deltaTime);
    }
    
    public bool IsInState<TState>() where TState : class, IState<TContext>
    {
        return currentState is TState;
    }

    public TState GetState<TState>() where TState : class, IState<TContext>
    {
        return currentState as TState;
    }
}
