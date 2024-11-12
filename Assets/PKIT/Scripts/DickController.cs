using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DickController : MonoBehaviour
{
    public Transform carTransform; // Reference to the car's transform
    public Transform dickTransform; // Reference to the dick (handle) transform
    public Vector3 leftLimit = new Vector3(-0.16f, 0f, 0f);  // Left position limit for the slider
    public Vector3 rightLimit = new Vector3(0.16f, 0f, 0f); // Right position limit for the slider
    private float maxRotation = 45f; // Max rotation of the car in degrees
    private float moveSpeed = 0.2f; // Speed of moving the handle


    private Vector3 initialDickPosition; // Store the initial position of the Dick (handle)
    private float initialCarRotationY; // Store the initial Y rotation of the car

    // Start is called before the first frame update
    void Start()
    {
        // Capture the initial positions and rotation
        initialDickPosition = dickTransform.localPosition;
        initialCarRotationY = carTransform.localRotation.eulerAngles.y; // Get the initial Y rotation of the car

        
    }

    
    // Update is called once per frame
    void Update()
    {
        /*
        // Move the handle left or right based on input
        if (Input.GetKey(KeyCode.A))
        {
            dickTransform.localPosition = new Vector3(
                Mathf.Clamp(dickTransform.localPosition.x - moveSpeed * Time.deltaTime, leftLimit.x, rightLimit.x),
                dickTransform.localPosition.y,
                dickTransform.localPosition.z
            );
        }
        else if (Input.GetKey(KeyCode.D))
        {
            dickTransform.localPosition = new Vector3(
                Mathf.Clamp(dickTransform.localPosition.x + moveSpeed * Time.deltaTime, leftLimit.x, rightLimit.x),
                dickTransform.localPosition.y,
                dickTransform.localPosition.z
            );
        }
        */

        // Calculate the slider's position ratio on the X-axis between leftLimit and rightLimit
        float dickPositionRatio = Mathf.InverseLerp(leftLimit.x, rightLimit.x, dickTransform.localPosition.x);

        // Map this position ratio to the car's rotation angle
        float carRotationY = Mathf.Lerp(-maxRotation, maxRotation, dickPositionRatio);

        // Update the car's Y rotation based on the slider's position
        carTransform.localRotation = Quaternion.Euler(0, initialCarRotationY + carRotationY, 0);

    }

    public void ControlCar()
    {
        // Calculate the handle's position as a value between 0 and 1 for left-to-right movement
        float handlePositionRatio = Mathf.InverseLerp(-0.16f, 0.16f, dickTransform.localPosition.x);

        // Calculate the corresponding car rotation based on the handle's movement
        float carRotationY = Mathf.Lerp(-maxRotation, maxRotation, handlePositionRatio);

        // Update the car's Y rotation based on the handle's position
        carTransform.localRotation = Quaternion.Euler(0, initialCarRotationY + carRotationY, 0);
    }
}

