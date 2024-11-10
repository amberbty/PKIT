using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    public Transform carTransform; // Reference to the car's transform
    public Transform wheelTransform; // Reference to the dick (handle) transform

    // Define the limits for the wheel's rotation around the Z-axis
    public float leftWheelLimitZ = -350f; // Maximum left rotation in degrees
    public float rightWheelLimitZ = 350f; // Maximum right rotation in degrees

    private float maxRotation = 45f; // Max rotation of the car in degrees
    private float moveSpeed = 100f; // Speed of moving the wheel when holding down A or D key


    private float initialWheelRotationY; // Store the initial position of the wheel
    private float initialCarRotationY; // Store the initial Y rotation of the car

    // Start is called before the first frame update
    void Start()
    {
        // Capture the initial rotations
        //initialWheelRotationY = wheelTransform.localRotation.eulerAngles.z;
        initialCarRotationY = carTransform.localRotation.eulerAngles.y; // Get the initial Y rotation of the car
    }

    // Update is called once per frame
    void Update()
    {
        // Move the handle left or right based on input
        if (Input.GetKey(KeyCode.A))
        {
            // Rotate left, clamping to the left limit
            float newZRotation = Mathf.Clamp(wheelTransform.eulerAngles.z + moveSpeed * Time.deltaTime, leftWheelLimitZ, rightWheelLimitZ);
            wheelTransform.eulerAngles = new Vector3(
                wheelTransform.eulerAngles.x,
                wheelTransform.eulerAngles.y,
                newZRotation
            );
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // Rotate right, clamping to the right limit
            float newZRotation = Mathf.Clamp(wheelTransform.eulerAngles.z - moveSpeed * Time.deltaTime, leftWheelLimitZ, rightWheelLimitZ);
            wheelTransform.eulerAngles = new Vector3(
                wheelTransform.eulerAngles.x,
                wheelTransform.eulerAngles.y,
                newZRotation
            );
        }


        // Calculate the wheel's current rotation angle around the Z-axis (local rotation)
        float wheelRotationZ = wheelTransform.localEulerAngles.z;

        /*
        // Convert wheel rotation to a range between -180 and 180 degrees to handle rotation direction
        if (wheelRotationZ > 180f)
        {
            wheelRotationZ -= 360f;
        }
        */

        // Calculate the wheel's rotation as a value between 0 and 1 for left-to-right movement
        float wheelRotationRatio = Mathf.InverseLerp(leftWheelLimitZ, rightWheelLimitZ, wheelRotationZ);

        // Calculate the corresponding car Y rotation based on the wheel's rotation ratio
        float carRotationY = Mathf.Lerp(-maxRotation, maxRotation, wheelRotationRatio);

        // Update the car's Y rotation based on the handle's rotation
        carTransform.localRotation = Quaternion.Euler(0, initialCarRotationY - carRotationY, 0);
    }
}
