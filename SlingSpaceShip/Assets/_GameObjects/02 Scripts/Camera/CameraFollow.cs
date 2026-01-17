using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    
    [Header("Follow Data")]
    [SerializeField] private Vector3 followTargetOffset;
    [SerializeField] private float smoothTime;

    [Header("Pivot Data")]
    [SerializeField] private Transform pivotT;
    [SerializeField] private Vector3 nearPivotPos;
    [SerializeField] private Vector3 farPivotPos;
    [SerializeField] private float pivotMoveSpeed;
    private Vector3 targetPivotPos;

    private Vector3 desiredPos;
    private Vector3 refVel;
    
    #region Singlton

    public static CameraFollow Instance;

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
    
    private void Start()
    {
        SetUp();
    }

    private void LateUpdate()
    {
        FollowTarget();
        MovePivot();
    }

    #region SetUp

    public void SetUp()
    {
        desiredPos = followTarget.position + followTargetOffset;
        transform.position = desiredPos;

        targetPivotPos = farPivotPos;
        pivotT.transform.localPosition = targetPivotPos;
    }

    #endregion

    #region Follow

    private void FollowTarget()
    {
        desiredPos = followTarget.position + followTargetOffset;
        transform.position = desiredPos;
    }

    #endregion

    #region Pivot

    private void MovePivot()
    {
        pivotT.transform.localPosition = Vector3.Lerp(pivotT.transform.localPosition, targetPivotPos, Time.unscaledDeltaTime * pivotMoveSpeed);
    }

    public void MovePivotToNearPos()
    {
        targetPivotPos = nearPivotPos;
    }

    public void MovePivotToFarPos()
    {
        targetPivotPos = farPivotPos;
    }

    #endregion
}
