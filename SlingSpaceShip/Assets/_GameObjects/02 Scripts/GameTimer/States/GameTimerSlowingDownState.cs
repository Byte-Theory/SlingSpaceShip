using UnityEngine;

public class GameTimerSlowingDownState : GameTimerBaseState
{
    private readonly IState<GameTimerStateContext> nextState;
    
    public GameTimerSlowingDownState(GameTimerStateContext gameTimerStateContext, IState<GameTimerStateContext> nextState) : base(gameTimerStateContext)
    {
        this.nextState = nextState;
    }

    public override void Enter(GameTimerStateContext context)
    {
        gameTimerStateContext.ResetData();
        
        gameTimerStateContext.stateStartFloat = Time.timeScale;
        gameTimerStateContext.stateEndFloat = Constants.GameTimerData.SlowDownTimeScale;
        
        postProcessingManager.AnimateTimeSlowDown();
        cameraFollow.MovePivotToNearPos();
    }
    
    public override void UpdateState(GameTimerStateContext context, float deltaTime)
    {
        gameTimerStateContext.timeElapsed += deltaTime;

        if (gameTimerStateContext.timeElapsed < gameTimerStateContext.stateDuration)
        {
            float fac = gameTimerStateContext.timeElapsed / gameTimerStateContext.stateDuration;
            float delta = gameTimerStateContext.stateAnimationCurve.Evaluate(fac);
            
            float timeScale = Mathf.Lerp(gameTimerStateContext.stateStartFloat, gameTimerStateContext.stateEndFloat, delta);
            
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
