using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Meta.XR;


public class DickController : MonoBehaviour
{
    public Rigidbody carRb;
    public Transform carTransform; // Reference to the car's transform
    public Transform dickTransform; // Reference to the dick (handle) transform
    public Vector3 leftLimit = new Vector3(-0.16f, 0f, 0f);  // Left position limit for the slider
    public Vector3 rightLimit = new Vector3(0.16f, 0f, 0f); // Right position limit for the slider
    private float maxRotation = 45f; // Max rotation of the car in degrees
    private float moveSpeed = 0.2f; // Speed of moving the handle
    public float returnSpeed = 1f;

    public float carSpeed = 5.0f;
    private float stopSmoothness = 5f;

    public bool isMoving = false;
    public bool isGrabbed = false;

    public OVRHand leftHandTracking;
    public OVRHand rightHandTracking;

    private Vector3 initialDickPosition; // Store the initial position of the Dick (handle)
    private float initialCarRotationY; // Store the initial Y rotation of the car

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the car transform is assigned
        if (carTransform == null)
        {
            carTransform = GameObject.FindGameObjectWithTag("Car").transform;  // Example: find the car by tag
        }

        // Capture the initial positions and rotation
        initialDickPosition = dickTransform.localPosition;
        initialCarRotationY = carTransform.localRotation.eulerAngles.y; // Get the initial Y rotation of the car
    }

    
    // Update is called once per frame
    void Update()
    {
        // Calculate the slider's position ratio on the X-axis between leftLimit and rightLimit
        float dickPositionRatio = Mathf.InverseLerp(leftLimit.x, rightLimit.x, dickTransform.localPosition.x);

        // Map this position ratio to the car's rotation angle
        float carRotationY = Mathf.Lerp(-maxRotation, maxRotation, dickPositionRatio);

        // Update the car's Y rotation based on the slider's position
        carTransform.localRotation = Quaternion.Euler(0, initialCarRotationY + carRotationY, 0);

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

    }
    public void ToggleCarMovement()
    {
        isMoving = !isMoving; // Toggle the movement state
    }
    private void MoveForward()
    {
        // Move the car forward along its current forward direction
        carRb.velocity = carTransform.rotation * Vector3.forward * carSpeed;
        //carRb.velocity = transform.forward * carSpeed;
    }

    private void SmoothReturnToInitialRotation()
    {
        // Smoothly return the handle to its initial rotation when released
        transform.localPosition = Vector3.Lerp(dickTransform.localPosition, initialDickPosition, Time.deltaTime * returnSpeed);
    }

    public void StartGrabbingHandle()
    {
        isGrabbed = true;
    }

    public void StopGrabbingHandle()
    {
        isGrabbed = false;
    }
}

