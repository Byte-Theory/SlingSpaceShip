using System;
using UnityEngine;

public class GameTimerManager : MonoBehaviour
{
    [Header("Animation Curve")]
    [SerializeField] private AnimationCurve timeScaleSlowDownCurve;
    [SerializeField] private float timeScaleChangeDur = 0.15f;

    // States
    private GameTimerState gameTimerState = GameTimerState.Unknown;
    private float timeElapsedSinceLastUpdate = 0.0f;
    
    // Time Scale Data
    private float timeScaleCurrrent = 1.0f;
    private float timeScaleTarget = 1.0f;
    
    private readonly float normalTimeScale = 1.0f;
    private readonly float slowDownTimeScale = 0.35f;

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
        UpdateStateTimer();
    }

    #region SetUp

    public void SetUp()
    {
        SetState(GameTimerState.Idle);
    }

    #endregion

    #region States

    private void SetState(GameTimerState newState)
    {
        if (newState == gameTimerState)
        {
            return;
        }

        SetStateData(newState);
        
        gameTimerState = newState;
        timeElapsedSinceLastUpdate = 0.0f;
    }

    private void SetStateData(GameTimerState newState)
    {
        if (newState == GameTimerState.Idle)
        {
            UpdateTimeScale(normalTimeScale, true);
        }
        else if (newState == GameTimerState.GoingBackToIdle)
        {
            UpdateTimeScale(normalTimeScale);
        }
        else if (newState == GameTimerState.SlowingDownTime)
        {
            UpdateTimeScale(slowDownTimeScale);
        }
    }

    private void UpdateStateTimer()
    {
        if (gameTimerState == GameTimerState.GoingBackToIdle)
        {
            timeElapsedSinceLastUpdate += Time.deltaTime;
            
            if (timeElapsedSinceLastUpdate < timeScaleChangeDur)
            {
                float fac = timeElapsedSinceLastUpdate / timeScaleChangeDur;
                float timeScale = Mathf.Lerp(timeScaleCurrrent, timeScaleTarget, fac);
                UpdateUnityTimeScale(timeScale);
            }
            else
            {
                UpdateUnityTimeScale(1.0f);
                SetState(GameTimerState.Idle);
            }
        }
        else if (gameTimerState == GameTimerState.SlowingDownTime)
        {
            timeElapsedSinceLastUpdate += Time.deltaTime;
            
            if (timeElapsedSinceLastUpdate < timeScaleChangeDur)
            {
                float fac = timeElapsedSinceLastUpdate / timeScaleChangeDur;
                float delta = timeScaleSlowDownCurve.Evaluate(fac);
                float timeScale = Mathf.Lerp(timeScaleCurrrent, timeScaleTarget, delta);
                UpdateUnityTimeScale(timeScale);
            }
            else
            {
                UpdateUnityTimeScale(slowDownTimeScale);
            }
        }
    }

    #endregion
    
    #region Update Time Scale

    private void UpdateTimeScale(float value, bool isInstant = false)
    {
        timeScaleCurrrent = Time.timeScale;
        timeScaleTarget = value;
        
        if (isInstant)
        {
            timeScaleCurrrent = timeScaleTarget;
            UpdateUnityTimeScale(timeScaleCurrrent);
        }
    }

    private void UpdateUnityTimeScale(float val)
    {
        Time.timeScale = val;
    }
    
    #endregion

    #region Helpers

    public void SetSlowTimeScale()
    {
        SetState(GameTimerState.SlowingDownTime);
    }
    
    public void SetNormalTimeScale()
    {
        SetState(GameTimerState.GoingBackToIdle);
    }

    #endregion
}
