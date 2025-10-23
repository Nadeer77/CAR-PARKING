using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider rearRight;
    [SerializeField] WheelCollider rearLeft;
    public Transform frontRightTransform;
    public Transform frontLeftTransform;
    public Transform rearRightTransform;
    public Transform rearLeftTransform;
    public GameObject[] WheelEffects;

    [SerializeField] float acceleration = 300f;
    public float breakingForce = 500f;
    public float maxTurnAngle = 15f;
    private float currentAcceleration = 0f;
    private float currentBreakForce = 0f;

    private float currentTurnAngle = 0f;

    void FixedUpdate()
    {
        Movement();
        SkidTrail();
    }
    void Movement()
    {
        // Getting Forward / Reverse Acceleration from Vertical Axis (using W and S Keys or UpArrow and DownArrow Keys)  
        currentAcceleration = acceleration * Input.GetAxis("Vertical");

        // By pressing Space , CurrentBrakeForce will have a value.
        if (Input.GetKey(KeyCode.Space))
        {
            currentBreakForce = breakingForce;
        }
        else
        {
            currentBreakForce = 0f;
        }

        // Apply Acceleration to Front Wheels.
        frontRight.motorTorque = currentAcceleration;
        frontLeft.motorTorque = currentAcceleration;

        // Apply Braking to all Wheels.
        frontRight.brakeTorque = currentBreakForce;
        frontLeft.brakeTorque = currentBreakForce;
        rearRight.brakeTorque = currentBreakForce;
        rearLeft.brakeTorque = currentBreakForce;

        // Turning the car left and right by the Horizontal axis (using A and D Keys or LeftArrow and RightArrow Keys) 
        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
        frontRight.steerAngle = currentTurnAngle;
        frontLeft.steerAngle = currentTurnAngle;

        UpdateWheel(frontRight, frontRightTransform);
        UpdateWheel(frontLeft, frontLeftTransform);
        UpdateWheel(rearRight, rearRightTransform);
        UpdateWheel(rearLeft, rearLeftTransform);
    }
    void UpdateWheel(WheelCollider collider, Transform transform)
    {
        //Get wheel collider state
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        //Set wheel transform state
        transform.position = position;
        transform.rotation = rotation;
    }
    void SkidTrail()
    {
        foreach (var wheelEffect in WheelEffects)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                wheelEffect.GetComponentInChildren<TrailRenderer>().emitting = true;
            }
            else
            {
                wheelEffect.GetComponentInChildren<TrailRenderer>().emitting = false;
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacles"))
        {
            UIManager.Instance.GameLoss();
        }  
    }
}
