using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 5.0f;
    public bool isMoving = false;

    public SteeringWheelController steeringWheel;  // Reference to Steering Wheel Controller

    private Rigidbody playerSpaceRb;

    void Start()
    {
        playerSpaceRb = GetComponent<Rigidbody>();
        playerSpaceRb.drag = 2f;
        playerSpaceRb.angularDrag = 2f;
    }

    void Update()
    {
        ApplySteering();
        if (isMoving)
        {
            MoveForward();
            
        }
        else
        {
            playerSpaceRb.velocity = Vector3.zero; // Stop the car when not moving
        }
    }

    private void MoveForward()
    {
        // Apply forward force based on Player Space's current forward direction
        Vector3 forwardDirection = transform.forward * speed;
        playerSpaceRb.velocity = forwardDirection;
    }

    private void ApplySteering()
    {
        // Get the steering angle and rotation speed from WheelController
        float steeringAngle = steeringWheel.GetSteeringAngle();
        float steeringRotationSpeed = steeringWheel.steeringRotationSpeed;

        // Scale the steering angle by steeringRotationSpeed to set the turning speed
        float turnAngle = steeringAngle * steeringRotationSpeed * Time.deltaTime;

        // Convert steering angle into rotation for the car
        Quaternion turnRotation = Quaternion.Euler(0, turnAngle, 0);

        // Rotate Player Space (and hence the car) around the Y-axis
        playerSpaceRb.MoveRotation(playerSpaceRb.rotation * turnRotation);
    }
}
