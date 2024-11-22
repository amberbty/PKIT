using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR;

public class WheelController : MonoBehaviour
{
    public float steeringRotationSpeed = 60f;  // Speed of steering rotation
    public float returnSpeed = 2f;             // Speed of returning to initial rotation
    public bool isMoving = false;             // Whether the car is moving
    public bool isGrabbed = false;            // Whether the wheel is being grabbed

    private Quaternion initialWheelLocalRotation;  // Fixed initial rotation of the wheel
    private float initialZ;                        // Fixed initial Z rotation (in degrees)

    private float carInitialYRotation;            // Initial Y rotation of the car (in degrees)
    public Rigidbody carRb;                       // Car Rigidbody for movement
    private float currentSteeringAngle = 0f;      // Current steering angle (-45 to 45 degrees)
    public float carSpeed = 5.0f;                 // Speed of the car
    private float stopSmoothness = 5f;            // Smoothness of stopping the car

    private Transform carTransform;              // Reference to the car's transform

    public CustomOneGrabRotateTransformer handTransformer;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the car transform is assigned
        if (carTransform == null)
        {
            carTransform = GameObject.FindGameObjectWithTag("Car").transform;  // Find car by tag
        }

        // Store the initial neutral rotation of the wheel
        initialWheelLocalRotation = transform.localRotation;
        initialZ = NormalizeAngle(initialWheelLocalRotation.eulerAngles.z);  // Store the fixed initial Z rotation

        carInitialYRotation = carTransform.localRotation.eulerAngles.y;  // Store the initial Y rotation of the car
    }

    // Update is called once per frame
    void Update() 
    {
        if (!isGrabbed)
        {
            //SmoothReturnToInitialRotation(currentZ);
        }

        if (isMoving)
        {
            MoveForward();
            // Detect the current rotation of the wheel
            float currentZ = NormalizeAngle(transform.localRotation.eulerAngles.z);

            // Calculate the steering angle based on the rotation difference along the Z-axis
            CalculateSteeringAngle(currentZ);

            // Apply the calculated steering angle to the car
            ApplySteeringAngleToCar();
        }
        else
        {
            // Smoothly stop the car
            carRb.velocity = Vector3.Lerp(carRb.velocity, Vector3.zero, Time.deltaTime * stopSmoothness);
        }
    }

    private void CalculateSteeringAngle(float currentZ)
    {
        // Calculate the angle between the initial rotation and the current rotation
        float rotationDifference = Mathf.DeltaAngle(initialZ, currentZ);

        // Optionally, consider smoothing the rotation to avoid abrupt changes
        currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, rotationDifference, Time.deltaTime * steeringRotationSpeed);

        // Clamp the rotation difference to the car's max steering angle
        currentSteeringAngle = Mathf.Clamp(currentSteeringAngle, -45f, 45f);
    }

    private void ApplySteeringAngleToCar()
    {
        float steeringAngle = -currentSteeringAngle;  // Negative to match wheel rotation
        carInitialYRotation += steeringAngle * Time.deltaTime;

        // Apply the cumulative steering angle to the car's local rotation
        carTransform.localRotation = Quaternion.Euler(0f, carInitialYRotation, 0f);
    }

    public void ToggleCarMovement()
    {
        isMoving = !isMoving; // Toggle the movement state
    }

    private void MoveForward()
    {
        // Move the car forward, preserving gravity
        Vector3 forwardForce = carTransform.forward * carSpeed;

        // Preserve Y-axis velocity (gravity)
        Vector3 currentVelocity = carRb.velocity;
        carRb.velocity = new Vector3(forwardForce.x, currentVelocity.y, forwardForce.z);
    }

    private void SmoothReturnToInitialRotation(float currentZ) //Makes wheel lock bug
    {
        // Get the current Z rotation of the wheel
        //float currentZ = NormalizeAngle(transform.localRotation.eulerAngles.z);

        // Calculate the shortest path back to the initial Z rotation
        float angleDifference = Mathf.DeltaAngle(currentZ, initialZ);

        // Interpolate smoothly toward the initial rotation
        float newZ = Mathf.Lerp(currentZ, currentZ + angleDifference, Time.deltaTime * returnSpeed);

        // Apply the updated rotation back to the wheel, preserving the X and Y rotation
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, newZ);
    }

    private float NormalizeAngle(float angle)
    {
        // Normalize angle to -180 to 180 range
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    public void StartGrabbingHandle()
    {
        isGrabbed = true;
        handTransformer.isGrabbed = true;
    }

    public void StopGrabbingHandle()
    {
        isGrabbed = false;
        handTransformer.isGrabbed = false;
    }
}
