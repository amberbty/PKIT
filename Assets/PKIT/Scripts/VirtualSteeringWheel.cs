using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR; // Adjust this import based on the SDK documentation

public class VirtualSteeringWheel : MonoBehaviour
{
    public float steeringRotationSpeed = 60f;
    public float returnSpeed = 2f;
    public float steeringLimitAngle = 100f;

    private float steeringAngle = 0f;
    private bool isGrabbed = false;


    public OVRHand leftHandTracking;
    public OVRHand rightHandTracking;

    // Assuming these represent the left and right hand grab tracking components in the Meta SDK
    //public MetaHandTracking leftHandTracking;
    //public MetaHandTracking rightHandTracking;

    private Quaternion initialRotation;
    private Quaternion initialHandRotation;

    void Start()
    {
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        CheckHandGrabs();

        if (isGrabbed)
        {
            RotateWheelWithHand();
        }
        else
        {
            ReturnToNeutral();
        }
        
    }

    private void CheckHandGrabs()
    {
        // Check if either hand is in a grabbing pose
        isGrabbed = (leftHandTracking && leftHandTracking.IsTracked) || (rightHandTracking && rightHandTracking.IsTracked);

        if (isGrabbed && initialHandRotation == Quaternion.identity)
        {
            // Set the initial rotation to start rotation calculations when grabbing starts
            initialHandRotation = leftHandTracking.IsTracked ? leftHandTracking.transform.rotation : rightHandTracking.transform.rotation;
        }
    }

    private void RotateWheelWithHand()
    {
        // Use the grabbing hand to calculate rotation
        OVRHand grabbingHand = leftHandTracking.IsTracked ? leftHandTracking : rightHandTracking;

        if (grabbingHand != null)
        {
            Quaternion handRotationDelta = Quaternion.Inverse(initialHandRotation) * grabbingHand.transform.rotation;
            steeringAngle += handRotationDelta.eulerAngles.y * steeringRotationSpeed * Time.deltaTime;
            steeringAngle = Mathf.Clamp(steeringAngle, -steeringLimitAngle, steeringLimitAngle);

            // Apply rotation to the wheel
            transform.localRotation = initialRotation * Quaternion.Euler(0, 0, -steeringAngle);
        }
    }

    private void ReturnToNeutral()
    {
        // Smoothly return to neutral position if not grabbed
        steeringAngle = Mathf.Lerp(steeringAngle, 0f, Time.deltaTime * returnSpeed);
        transform.localRotation = initialRotation * Quaternion.Euler(0, 0, -steeringAngle);
    }

    public float GetSteeringAngle()
    {
        return steeringAngle;
    }
}
