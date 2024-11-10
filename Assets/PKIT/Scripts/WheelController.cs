using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    public Transform wheelTransform; // Reference to the wheel transform

    public GameObject car;

    // Define the limits for the wheel's rotation around the Z-axis
    public float leftWheelLimitZ = 350f; // Maximum left rotation in degrees
    public float rightWheelLimitZ = -350f; // Maximum right rotation in degrees

    private float maxRotation = 45f; // Max rotation of the car in degrees
    private float steerSpeed = 100f; // Speed of moving the wheel when holding down A or D key
    float newSteeringWheelZ;
    public float returnSpeed = 2f;


    private float initialWheelRotationZ; // Store the initial position of the wheel
    private float initialCarRotationY; // Store the initial Y rotation of the car

    // Start is called before the first frame update
    void Start()
    {
        // Capture the initial rotations
        //initialWheelRotation = wheelTransform.localRotation.eulerAngles.z;
        initialCarRotationY = car.transform.localRotation.eulerAngles.y; // Get the initial Y rotation of the car
    }

    // Update is called once per frame
    void Update()
    {
        // Get input for rotating the steering wheel (left or right)
        newSteeringWheelZ = wheelTransform.localEulerAngles.z;

        // Move the handle left or right based on input
        if (Input.GetKey(KeyCode.A))
        {
            // Rotate the steering wheel left, clamping to the max left rotation
            newSteeringWheelZ += steerSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // Rotate the steering wheel right, clamping to the max right rotation
            newSteeringWheelZ -= steerSpeed * Time.deltaTime;
        }


        // Calculate the wheel's current rotation angle around the Z-axis (local rotation)
        float wheelRotationZ = wheelTransform.localEulerAngles.z;
        

        // Convert newSteeringWheelZ to the -180 to 180 range
        if (newSteeringWheelZ > 180f) newSteeringWheelZ -= 360f;

        // Apply the rotation to the steering wheel transform
        wheelTransform.localEulerAngles = new Vector3(
            wheelTransform.localEulerAngles.x,
            wheelTransform.localEulerAngles.y,
            newSteeringWheelZ
        );

        // Calculate a normalized rotation value (-1 to 1) based on steering wheel rotation
        float steeringRatio = Mathf.InverseLerp(leftWheelLimitZ, rightWheelLimitZ, newSteeringWheelZ); 

        // Calculate the car's rotation based on the steering wheel's position
        float carRotationY = steeringRatio * maxRotation;

        // Apply the calculated car rotation, adjusted by the initial Y rotation
        car.transform.localRotation = Quaternion.Euler(0, initialCarRotationY + carRotationY, 0);

        ReturnToNeutral();
    }

    private void ReturnToNeutral()
    {
        if (!(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            // Smoothly return to neutral position if not grabbed
            newSteeringWheelZ = Mathf.Lerp(newSteeringWheelZ, 0f, Time.deltaTime * returnSpeed);

            transform.localRotation = Quaternion.Euler(0, 0, initialWheelRotationZ - newSteeringWheelZ);
        }
    }
}