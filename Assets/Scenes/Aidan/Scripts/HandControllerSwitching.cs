using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControllerSwitching : MonoBehaviour
{
    [SerializeField] private GameObject rightController = null;
    [SerializeField] private GameObject leftController = null;
    [SerializeField] private GameObject rightHand = null;
    [SerializeField] private GameObject leftHand = null;
    private bool isControllerActive = true;
  
    private void Update()
    {
        if (OVRPlugin.GetHandTrackingEnabled())
        {
            if (isControllerActive)
            {
                SetHandsActive(true);
                SetControllersActive(false);
                isControllerActive = false;
            }
        }
        else
        {
            if (!isControllerActive)
            {
                SetHandsActive(false);
                SetControllersActive(true);
                isControllerActive = true;
            }
        }
    }

    private void SetHandsActive(bool value)
    {
        rightHand.SetActive(value);
        leftHand.SetActive(value);
    }

    private void SetControllersActive(bool value)
    {
        rightController.SetActive(value);
        leftController.SetActive(value);
    }
}
