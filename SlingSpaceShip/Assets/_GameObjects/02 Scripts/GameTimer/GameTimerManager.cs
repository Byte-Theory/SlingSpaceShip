using System;
using UnityEngine;

public class GameTimerManager : MonoBehaviour
{
    [Header("Animation Curve")]
    [SerializeField] private AnimationCurve timeScaleSlowDownCurve;
    [SerializeField] private float timeScaleChangeDur = 0.15f;

    // States
    private StateMachine<GameTimerStateContext> stateMachine;
    private GameTimerStateContext gameTimerStateContext;
    private GameTimerIdleState gameTimerIdleState;
    private GameTimerBackToNormalState gameTimerBackToNormalState;
    private GameTimerSlowingDownState gameTimerSlowingDownState;
    
    #region Singlton

    public static GameTimerManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    #endregion
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.UpdateState(Time.deltaTime);
    }

    #region SetUp

    public void SetUp()
    {
        SetUpStateMachine();
        SetIdleState();
    }

    #endregion

    #region States

    private void SetUpStateMachine()
    {
        gameTimerStateContext = new GameTimerStateContext
        {
            timeElapsed = 0.0f,
            stateDuration = timeScaleChangeDur,
            stateAnimationCurve = timeScaleSlowDownCurve
        };

        stateMachine = new StateMachine<GameTimerStateContext>(gameTimerStateContext);

        gameTimerIdleState = new GameTimerIdleState(gameTimerStateContext);
        gameTimerBackToNormalState = new GameTimerBackToNormalState(gameTimerStateContext, gameTimerIdleState);
        gameTimerSlowingDownState = new GameTimerSlowingDownState(gameTimerStateContext, null);

        gameTimerStateContext.RequestStateChange = stateMachine.SetState;
    }

    private void SetIdleState()
    {
        stateMachine.SetState(gameTimerIdleState);
    }

    #endregion
    
    #region Helpers

    public void SetSlowTimeScale()
    {
        stateMachine.SetState(gameTimerSlowingDownState);
    }
    
    public void SetNormalTimeScale()
    {
        stateMachine.SetState(gameTimerBackToNormalState);
    }

    #endregion

    #region Getter

    public bool IsInSlowDownState()
    {
        var slowState = stateMachine.GetState<GameTimerSlowingDownState>();
        return slowState != null;
    }

    #endregion
}
