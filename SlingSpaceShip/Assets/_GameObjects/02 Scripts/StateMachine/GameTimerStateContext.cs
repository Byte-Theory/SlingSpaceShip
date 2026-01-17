using System;
using UnityEngine;

public class GameTimerStateContext
{
    public float timeElapsed;
    public float stateDuration;

    public float stateStartFloat;
    public float stateEndFloat;
    
    public AnimationCurve stateAnimationCurve;
    
    public Action<IState<GameTimerStateContext>> RequestStateChange;

    public void ResetData()
    {
        timeElapsed = 0;
        stateStartFloat = 0;
        stateEndFloat = 0;
    }
}
