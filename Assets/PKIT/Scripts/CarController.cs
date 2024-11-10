using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // To use UnityEvent
using Meta.XR; // Adjust this import based on the SDK documentation

public class CarController : MonoBehaviour
{
    public float speed = 5.0f;
    public float turnSpeed = 5.0f; // Stable turn speed multiplier
    public bool isMoving = false;

    public GameObject steeringWheel;  // Reference to SteeringWheel

    // Steering Wheel settings
    public float steeringRotationSpeed = 60f;
    private float previousSteeringAngle = 0f; // To store the previous steering angle
    public float maxRotationSpeed = 100f; // Maximum allowed rotation speed (degrees per second)
    public float returnSpeed = 2f;
    public float steeringLimitAngle = 100f;

    private float steeringAngle = 0f;
    private bool isGrabbed = false;

    public OVRHand leftHandTracking;
    public OVRHand rightHandTracking;

    private Rigidbody carRb;
    private Quaternion initialRotation;
    private Quaternion initialHandRotation;

    //public UnityEvent onButtonPress; // Event to be triggered by the button press

    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.drag = 2f;
        carRb.angularDrag = 2f;
        initialRotation = transform.localRotation;

        // Optionally set initial rotation of the steering wheel if needed
        if (steeringWheel != null)
        {
            initialRotation = steeringWheel.transform.localRotation;
        }
    }

    void Update()
    {
        CheckHandGrabs();

        if (isGrabbed)
        {
            RotateWheelWithHand();
        }
        else
        {
            //ReturnToNeutral();
        }

        ApplySteering();

        if (isMoving)
        {
            MoveForward();
        }
        else
        {
            carRb.velocity = Vector3.zero; // Stop the car when not moving
        }
    }

    private void CheckHandGrabs()
    {
        // Check if either hand is in a grabbing pose
        isGrabbed = (leftHandTracking && leftHandTracking.IsTracked) || (rightHandTracking && rightHandTracking.IsTracked);

        if (isGrabbed && initialHandRotation == Quaternion.identity)
        {
            // Set the initial rotation to start rotation calculations when grabbing starts
            initialHandRotation = leftHandTracking.IsTracked ? leftHandTracking.transform.rotation : rightHandTracking.transform.rotation;
        }
    }

    private void RotateWheelWithHand()
    {
        // Use the grabbing hand to calculate rotation
        OVRHand grabbingHand = leftHandTracking.IsTracked ? leftHandTracking : rightHandTracking;

        if (grabbingHand != null)
        {
            // Calculate the rotation difference
            Quaternion handRotationDelta = Quaternion.Inverse(initialHandRotation) * grabbingHand.transform.rotation;
            float handRotationZDelta = handRotationDelta.eulerAngles.z;

            // Calculate the speed at which the hand is rotating
            float steerSpeed = Mathf.Abs(handRotationZDelta * steeringRotationSpeed * Time.deltaTime);

            // Limit the rotation speed
            steerSpeed = Mathf.Min(steerSpeed, maxRotationSpeed * Time.deltaTime); // Limit to max speed

            // Update the steering angle smoothly
            steeringAngle = Mathf.Lerp(previousSteeringAngle, previousSteeringAngle + handRotationZDelta, steerSpeed);

            // Clamp the steering angle to the desired limit
            steeringAngle = Mathf.Clamp(steeringAngle, -steeringLimitAngle, steeringLimitAngle);

            // Update the previous steering angle for the next frame
            previousSteeringAngle = steeringAngle;

            // Apply rotation to the steering wheel
            if (steeringWheel != null)
            {
                steeringWheel.transform.localRotation = initialRotation * Quaternion.Euler(0, 0, -steeringAngle);
            }
        }
    }

    /*
    private void ReturnToNeutral()
    {
        // Smoothly return to neutral position if not grabbed
        steeringAngle = Mathf.Lerp(steeringAngle, 0f, Time.deltaTime * returnSpeed);

        if (steeringWheel != null)
        {
            steeringWheel.transform.localRotation = initialRotation * Quaternion.Euler(0, 0, -steeringAngle);
        }
    }
    */

    private void ApplySteering()
    {
        // Scale the steering angle by steeringRotationSpeed to set the turning speed
        float turnAngle = steeringAngle * turnSpeed * Time.deltaTime;

        // Rotate the car around the Y-axis
        carRb.MoveRotation(carRb.rotation * Quaternion.Euler(0, turnAngle, 0));
    }

    public void ToggleCarMovement()
    {
        isMoving = !isMoving; // Toggle the movement state
    }

    private void MoveForward()
    {
        // Move the car forward along its current forward direction
        carRb.velocity = transform.forward * speed;

        // Apply forward force based on Player Space's current forward direction
        // Vector3 forwardDirection = transform.forward * speed;
        // carRb.velocity = forwardDirection;
    }
    public float GetSteeringAngle()
    {
        return steeringAngle;
    }

}
