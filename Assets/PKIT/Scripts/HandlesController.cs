using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR; // Adjust this import based on the SDK documentation

public class HandlesController : MonoBehaviour
{
    public float steeringRotationSpeed = 60f;
    public float returnSpeed = 2f;

    public bool isGrabbed = false;

    public Quaternion initialHandlesLocalRotation;  // Initial local rotation of the bike handles when grabbing starts
    public Quaternion currentHandlesLocalRotation;  // Current local rotation of the bike handles

    private float currentSteeringAngle = 0f;    // The current steering angle of the car (from 0 to max turn angle)
    private float initialY;                          // Initial y rotation of the bike handles (in degrees)

    private float carInitialYRotation;               // Initial y rotation of the car (in degrees)

    public OVRHand leftHandTracking;
    public OVRHand rightHandTracking;

    private Transform carTransform; // The car transform (you can use a car steering component or transform to apply the steering)

    void Start()
    {
        // Ensure the car transform is assigned
        if (carTransform == null)
        {
            carTransform = GameObject.FindGameObjectWithTag("Car").transform;  // Example: find the car by tag
        }

        // Store the initial neutral rotation of the handles (to calculate rotation delta later)
        initialHandlesLocalRotation = transform.localRotation;
        initialY = initialHandlesLocalRotation.eulerAngles.y;

        carInitialYRotation = carTransform.localRotation.eulerAngles.y;

    }

    void Update()
    {
        // Detect the current rotation of the bike handles
        float currentY = transform.localRotation.eulerAngles.y;

        // Calculate the steering angle based on the rotation difference along the y-axis
        CalculateSteeringAngle(currentY);

        // Apply the calculated steering angle to the car
        ApplySteeringAngleToCar();
    }

    private void CalculateSteeringAngle(float currentY)
    {
        // Calculate the angle between the initial rotation and the current rotation
        float rotationDifference = Mathf.DeltaAngle(initialY, currentY);

        // Optionally, consider smoothing the rotation to avoid abrupt changes
        currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, rotationDifference, Time.deltaTime * steeringRotationSpeed);

        // Map the rotation difference to the steering angle of the car (e.g., clamp it to a max steering angle)
        currentSteeringAngle = Mathf.Clamp(currentSteeringAngle, -45f, 45f);
    }

    private void ApplySteeringAngleToCar()
    {
        // Apply the calculated steering angle to the car's steering system (assuming it's based on local rotation)
        // This could be done by rotating the car or applying torque depending on your car's steering system

        carTransform.localRotation = Quaternion.Euler(0f, carInitialYRotation + currentSteeringAngle, 0f);
    }
}