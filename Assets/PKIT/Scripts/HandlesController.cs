using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR; // Adjust this import based on the SDK documentation

public class HandlesController : MonoBehaviour
{
    public float steeringRotationSpeed = 60f;
    public float returnSpeed = 2f;
    public float steeringLimitAngle = 100f;

    private float steeringAngle = 0f;
    public bool isGrabbed = false;


    public OVRHand leftHandTracking;
    public OVRHand rightHandTracking;

    private Quaternion initialRotation;
    private Quaternion initialHandRotation;

    void Start()
    {
        initialRotation = transform.localRotation;
        //isGrabbed = false;  // Set `isGrabbed` to false only at the start
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

        /*bool leftHandGrabbed = leftHandTracking && leftHandTracking.IsTracked && leftHandTracking.GetFingerIsPinching(OVRHand.HandFinger.Index);
        bool rightHandGrabbed = rightHandTracking && rightHandTracking.IsTracked && rightHandTracking.GetFingerIsPinching(OVRHand.HandFinger.Index);

        if ((leftHandGrabbed || rightHandGrabbed) && !isGrabbed)
        {
            isGrabbed = true;
            initialHandRotation = leftHandGrabbed ? leftHandTracking.transform.rotation : rightHandTracking.transform.rotation;
        }
        else if (!leftHandGrabbed && !rightHandGrabbed)
        {
            isGrabbed = false;
        }
        */


        /*
        // Check if either hand is in a grabbing pose
        isGrabbed = (leftHandTracking && leftHandTracking.IsTracked && leftHandTracking.GetFingerIsPinching(OVRHand.HandFinger.Index)) ||
                (rightHandTracking && rightHandTracking.IsTracked && rightHandTracking.GetFingerIsPinching(OVRHand.HandFinger.Index));
        //isGrabbed = (leftHandTracking && leftHandTracking.IsTracked) || (rightHandTracking && rightHandTracking.IsTracked);

        if (isGrabbed && initialHandRotation == Quaternion.identity)
        {
            // Set the initial rotation to start rotation calculations when grabbing starts
            initialHandRotation = leftHandTracking.IsTracked ? leftHandTracking.transform.rotation : rightHandTracking.transform.rotation;
        }
        */
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
            transform.localRotation = initialRotation * Quaternion.Euler(0, -steeringAngle, 0);
        }
    }

    private void ReturnToNeutral()
    {
        // Smoothly return to neutral position if not grabbed
        steeringAngle = Mathf.Lerp(steeringAngle, 0f, Time.deltaTime * returnSpeed);
        transform.localRotation = initialRotation * Quaternion.Euler(0, -steeringAngle, 0);
    }

    public float GetSteeringAngle()
    {
        return steeringAngle;
    }
}
