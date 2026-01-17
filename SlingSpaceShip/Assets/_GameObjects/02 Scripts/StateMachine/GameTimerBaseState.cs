public abstract class GameTimerBaseState : IState<GameTimerStateContext>
{
    protected readonly GameTimerStateContext gameTimerStateContext;

    protected PostProcessingManager postProcessingManager;
    
    protected GameTimerBaseState(GameTimerStateContext gameTimerStateContext)
    {
        this.gameTimerStateContext = gameTimerStateContext;
        postProcessingManager = PostProcessingManager.Instance;
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
