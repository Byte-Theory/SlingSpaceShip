public abstract class GameTimerBaseState : IState<GameTimerStateContext>
{
    protected readonly GameTimerStateContext gameTimerStateContext;

    protected GameTimerBaseState(GameTimerStateContext gameTimerStateContext)
    {
        this.gameTimerStateContext = gameTimerStateContext;
    }
    
    public virtual void Enter(GameTimerStateContext context)
    {
        gameTimerStateContext.ResetData();
    }

    public abstract void UpdateState(GameTimerStateContext context, float deltaTime);

    public virtual void Exit(GameTimerStateContext context)
    {
        
    }
}
