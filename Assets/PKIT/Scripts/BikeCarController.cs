using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Meta.XR;

public class BikeCarController : MonoBehaviour
{
    public float speed = 5.0f;
    public float turnSpeed = 5.0f; // Stable turn speed multiplier
    public bool isMoving = false;
    public float stopSmoothness = 5;

    public HandlesController handlesController;  // Reference to handlesController

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
        ApplySteering();

        if (isMoving)
        {
            MoveForward();
        }
        else
        {
            carRb.velocity = Vector3.Lerp(carRb.velocity, Vector3.zero, Time.deltaTime * stopSmoothness);
            //carRb.velocity = Vector3.zero; // Stop the car when not moving
        }
    }


    public void ToggleCarMovement()
    {
        isMoving = !isMoving; // Toggle the movement state
    }

    private void MoveForward()
    {
        // Move the car forward along its current forward direction
        carRb.velocity = transform.forward * speed;

    }

    private void ApplySteering()
    {
        // Get the steering angle and rotation speed from WheelController
        float steeringAngle = handlesController.GetSteeringAngle();

        // Scale the steering angle by steeringRotationSpeed to set the turning speed
        float turnAngle = steeringAngle * turnSpeed * Time.deltaTime;

        //Rotate the car around the Y-axis
        //transform.Rotate(Vector3.up, turnAngle);
        //transform.localRotation = initialRotation * Quaternion.Euler(0, -steeringAngle, 0);
        carRb.MoveRotation(carRb.rotation * Quaternion.Euler(0, turnAngle, 0));
    }
}
