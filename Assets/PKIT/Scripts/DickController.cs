using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DickController : MonoBehaviour
{
    public Transform carTransform; // Reference to the car's transform
    public Transform dickTransform; // Reference to the dick (handle) transform
    public Vector3 leftLimit = new Vector3(-0.71f, 1.035779f, 0.3951f);  // Left position limit for the Dick
    public Vector3 rightLimit = new Vector3(-0.444f, 1.035779f, 0.3951f); // Right position limit for the Dick
    private float maxRotation = 45f; // Max rotation of the car in degrees
    private float moveSpeed = 0.2f; // Speed of moving the handle when holding down A or D key


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

        // Calculate the handle's position as a value between 0 and 1 for left-to-right movement
        float handlePositionRatio = Mathf.InverseLerp(leftLimit.x, rightLimit.x, dickTransform.localPosition.x);

        // Calculate the corresponding car rotation based on the handle's movement
        float carRotationY = Mathf.Lerp(-maxRotation, maxRotation, handlePositionRatio);

        // Update the car's Y rotation based on the handle's position
        carTransform.localRotation = Quaternion.Euler(0, initialCarRotationY + carRotationY, 0);
    }
}

