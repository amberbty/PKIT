using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 5.0f;
    public float turnSpeed = 5.0f; // Stable turn speed multiplier
    public bool isMoving = false;

    public VirtualSteeringWheel virtualSteeringWheel;  // Reference to virtualSteeringWheel

    private Rigidbody carRb;
    private Quaternion initialRotation;

    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.drag = 2f;
        carRb.angularDrag = 2f;
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.A)) {
            isMoving = !isMoving;
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

    private void MoveForward()
    {
        // Move the car forward along its current forward direction
        carRb.velocity = transform.forward * speed;


        // Apply forward force based on Player Space's current forward direction
        // Vector3 forwardDirection = transform.forward * speed;
        // carRb.velocity = forwardDirection;
    }

    private void ApplySteering()
    {
        // Get the steering angle and rotation speed from WheelController
        float steeringAngle = virtualSteeringWheel.GetSteeringAngle();

        // Scale the steering angle by steeringRotationSpeed to set the turning speed
        float turnAngle = steeringAngle * turnSpeed * Time.deltaTime;

        //Rotate the car around the Y-axis
        //transform.Rotate(Vector3.up, turnAngle);
        //transform.localRotation = initialRotation * Quaternion.Euler(0, -steeringAngle, 0);
        carRb.MoveRotation(carRb.rotation * Quaternion.Euler(0, turnAngle, 0));
    }
}
