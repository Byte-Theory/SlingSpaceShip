using UnityEngine;

public class GameTimerIdleState : GameTimerBaseState
{
    public GameTimerIdleState(GameTimerStateContext gameTimerStateContext) : base(gameTimerStateContext)
    {
    }

    public override void Enter(GameTimerStateContext context)
    {
        gameTimerStateContext.ResetData();

        Time.timeScale = 1.0f;
    }
    
    public override void UpdateState(GameTimerStateContext context, float deltaTime)
    {
    }
}
