using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Container")]
    [SerializeField] private GameObject playerContainer;

    [Header("Shooting Data")]
    [SerializeField] private Vector2 shootingForceRange;
    
    // Rotation
    private float finalRotationAngleZ;
    private Quaternion finalRotation;
    
    // Physics
    private Rigidbody rb;
    
    // Input
    private bool isUserInputActive = false;

    private void OnEnable()
    {
        UserInput.UserTapStarted += OnUserTapStarted;
        UserInput.UserTapped += OnUserTapped;
        UserInput.UserTapEnded += OnUserEnded;
    }

    private void OnDisable()
    {
        UserInput.UserTapStarted -= OnUserTapStarted;
        UserInput.UserTapped -= OnUserTapped;
        UserInput.UserTapEnded -= OnUserEnded;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        EnablePhysics(true);
    }

    private void Update()
    {
        SmoothRotatePlayer();
        UpdateContainerScale();
    }

    #region Input

    private void OnUserTapStarted()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        EnablePhysics(false);
        
        isUserInputActive = true;
    }

    private void OnUserTapped(Vector2 lookDirection)
    {
        CalcRotation(lookDirection);
    }

    private void OnUserEnded(Vector2 direction, float magnitude)
    {
        EnablePhysics(true);
        
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        ShootPlayerInForward(magnitude);
        
        isUserInputActive = false;
    }

    #endregion

    #region Physics

    private void EnablePhysics(bool enable)
    {
        rb.isKinematic = !enable;
    }

    private void ShootPlayerInForward(float magnitude)
    {
        float forceMag = Mathf.Lerp(shootingForceRange.x, shootingForceRange.y, magnitude / 250f);
        Vector3 direction = transform.right;
        
        Vector3 force = direction * forceMag;
        
        rb.AddForce(force, ForceMode.VelocityChange);
    }

    #endregion

    #region Transfrom

    private void CalcRotation(Vector3 direction)
    {
        finalRotationAngleZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        finalRotation = Quaternion.Euler(0.0f, 0.0f, finalRotationAngleZ);
    }

    private void SmoothRotatePlayer()
    {
        if (isUserInputActive)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, Time.deltaTime * 20.0f);
        }
        else
        {
            Vector3 velocity = rb.linearVelocity;

            if (velocity.x >= 0 && velocity.x < 0.1f)
            {
                finalRotationAngleZ = 0.0f;
            }
            else if (velocity.x < 0 && velocity.x >= -0.1f)
            {
                finalRotationAngleZ = 180.0f;
            }
            else
            {
                Vector3 direction = velocity.normalized;

                finalRotationAngleZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            }

            finalRotation = Quaternion.Euler(0.0f, 0.0f, finalRotationAngleZ);
            transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, Time.deltaTime * 50.0f);
        }
    }

    private void UpdateContainerScale()
    {
        if (finalRotationAngleZ <= 90.0f && finalRotationAngleZ >= -90.0f)
        {
            playerContainer.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        else
        {
            playerContainer.transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);
        }
    }

    #endregion
}
