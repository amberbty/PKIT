using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

    public float speed = 20.0f;
    public float turnSpeed = 20.0f;
    public float horizontalInput;
    public float forwardInput;

    public Transform steeringWheel;
    public float steeringLimitAngle = 100f;
    public float steeringRotationSpeed = 60f;

    private Rigidbody carRb;

    // Start is called before the first frame update
    void Start()
    {
        carRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        UpdateSteerWheel(horizontalInput);

        if(forwardInput != 0f)
        {
            // Move the vehicle forward based on vertical input
            //carRb.AddForce(transform.forward * Time.deltaTime * speed * forwardInput, ForceMode.Impulse);
            transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);
        }
        transform.Rotate(Vector3.up, turnSpeed * horizontalInput * Time.deltaTime);

    }

    public void UpdateSteerWheel(float horizontalInput)
    {
        if (horizontalInput != 0f)
        {
            steeringWheel.Rotate(Vector3.forward, -horizontalInput * steeringRotationSpeed * Time.deltaTime);
        }
        if (horizontalInput == 0f && steeringWheel.localEulerAngles.z > 5f)
        {
            steeringWheel.localEulerAngles = new Vector3(steeringWheel.localEulerAngles.x,
                                                        steeringWheel.localEulerAngles.y,
                                                        0f);
        }
    }
}
