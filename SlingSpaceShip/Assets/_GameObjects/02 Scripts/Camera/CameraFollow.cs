using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    
    [Header("Follow Data")]
    [SerializeField] private Vector3 followTargetOffset;
    [SerializeField] private float smoothTime;

    private Vector3 desiredPos;
    private Vector3 refVel;
    
    private void Start()
    {
        SetUp();
    }

    private void LateUpdate()
    {
        FollowTarget();
    }

    #region SetUp

    public void SetUp()
    {
        desiredPos = followTarget.position + followTargetOffset;
        transform.position = desiredPos;
    }

    #endregion

    #region Follow

    private void FollowTarget()
    {
        desiredPos = followTarget.position + followTargetOffset;
        transform.position = desiredPos;
    }

    #endregion
}
