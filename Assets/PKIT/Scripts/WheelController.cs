using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR; 

public class WheelController : MonoBehaviour
{
    public float steeringRotationSpeed = 60f;
    public float returnSpeed = 2f;

    public bool isMoving = false;
    public bool isGrabbed = false;

    public Quaternion initialWheelLocalRotation;  // Initial local rotation of the wheel when grabbing starts


    private float initialZ;                          // Initial z rotation of the wheel (in degrees)

    private float carInitialYRotation;               // Initial y rotation of the car (in degrees)
    public Rigidbody carRb;
    private float currentSteeringAngle = 0f;    // The current steering angle of the car (from 0 to max turn angle)
    public float carSpeed = 5.0f;
    private float stopSmoothness = 5f;

    public OVRHand leftHandTracking;
    public OVRHand rightHandTracking;

    private Transform carTransform; // The car transform (you can use a car steering component or transform to apply the steering)


    // Start is called before the first frame update
    void Start()
    {
        // Ensure the car transform is assigned
        if (carTransform == null)
        {
            carTransform = GameObject.FindGameObjectWithTag("Car").transform;  // Example: find the car by tag
        }


        // Store the initial neutral rotation of the wheel (to calculate rotation delta later)
        initialWheelLocalRotation = transform.localRotation;
        initialZ = initialWheelLocalRotation.eulerAngles.z;

        carInitialYRotation = carTransform.localRotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Detect the current rotation of the wheel
        float currentZ = transform.localRotation.eulerAngles.z;
        // Convert newSteeringWheelZ to the -180 to 180 range
        if (currentZ > 180f) currentZ -= 360f;

        // Calculate the steering angle based on the rotation difference along the y-axis
        CalculateSteeringAngle(currentZ);

        // Apply the calculated steering angle to the car
        ApplySteeringAngleToCar();

        if (!isGrabbed)
        {
            SmoothReturnToInitialRotation();
        }

        if (isMoving)
        {
            MoveForward();
        }
        else
        {
            carRb.velocity = Vector3.Lerp(carRb.velocity, Vector3.zero, Time.deltaTime * stopSmoothness);
        }


        /*
        // Get input for rotating the steering wheel (left or right)
        newSteeringWheelZ = wheelTransform.localEulerAngles.z;
        

        // Convert newSteeringWheelZ to the -180 to 180 range
        //if (newSteeringWheelZ > 180f) newSteeringWheelZ -= 360f;

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

    */
    }

    private void CalculateSteeringAngle(float currentZ)
    {
        // Calculate the angle between the initial rotation and the current rotation
        float rotationDifference = Mathf.DeltaAngle(initialZ, currentZ);

        // Optionally, consider smoothing the rotation to avoid abrupt changes
        currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, rotationDifference, Time.deltaTime * steeringRotationSpeed);

        // Map the rotation difference to the steering angle of the car (e.g., clamp it to a max steering angle)
        currentSteeringAngle = Mathf.Clamp(currentSteeringAngle, -45f, 45f);
    }

    private void ApplySteeringAngleToCar()
    {
        carInitialYRotation += currentSteeringAngle * Time.deltaTime;

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

    private void SmoothReturnToInitialRotation()
    {
        // Smoothly return the handle to its initial rotation when released
        transform.localRotation = Quaternion.Slerp(transform.localRotation, initialWheelLocalRotation, Time.deltaTime * returnSpeed);
    }

    public void StartGrabbingHandle()
    {
        isGrabbed = true;
    }

    public void StopGrabbingHandle()
    {
        isGrabbed = false;
    }



    /*
     * private void ReturnToNeutral()
    {
        if (!(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            // Smoothly interpolate towards neutral rotation
            newSteeringWheelZ = Mathf.Lerp(newSteeringWheelZ, 0f, Time.deltaTime * returnSpeed);

            transform.localRotation = Quaternion.Euler(0, 0, newSteeringWheelZ);
        }
    }
    */


}