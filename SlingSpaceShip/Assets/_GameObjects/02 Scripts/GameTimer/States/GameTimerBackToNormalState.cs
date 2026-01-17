using UnityEngine;

public class GameTimerBackToNormalState : GameTimerBaseState
{
    private readonly IState<GameTimerStateContext> nextState;

    public GameTimerBackToNormalState(GameTimerStateContext gameTimerStateContext, IState<GameTimerStateContext> nextState) : base(gameTimerStateContext)
    {
        this.nextState = nextState;
    }
    
    public override void Enter(GameTimerStateContext context)
    {
        gameTimerStateContext.ResetData();
        
        gameTimerStateContext.stateStartFloat = Time.timeScale;
        gameTimerStateContext.stateEndFloat = 1.0f;
        
        postProcessingManager.AnimateTimeBackToNormal();
    }
    
    public override void UpdateState(GameTimerStateContext context, float deltaTime)
    {
        gameTimerStateContext.timeElapsed += deltaTime;

        if (gameTimerStateContext.timeElapsed < (gameTimerStateContext.stateDuration * 0.1f))
        {
            float fac = gameTimerStateContext.timeElapsed / (gameTimerStateContext.stateDuration * 0.1f);
            float timeScale = Mathf.Lerp(gameTimerStateContext.stateStartFloat, gameTimerStateContext.stateEndFloat, fac);
            
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = timeScale * 0.02f;
        }
        else
        {
            float timeScale = gameTimerStateContext.stateEndFloat;
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = timeScale * 0.02f;
            
            gameTimerStateContext.RequestStateChange?.Invoke(nextState);
        }
    }
}
