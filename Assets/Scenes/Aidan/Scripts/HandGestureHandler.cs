using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HandGestureHandler : MonoBehaviour
{
    [SerializeField] private Transform thumbTipTransform = null;
    [SerializeField] private Transform middleFingerTipTransform = null;
    [SerializeField] private float minDistance = 0.01f;
    [SerializeField] private HandTriggerHandler triggerHandler = null;

    // Update is called once per frame
    void Update()
    {
        // Calculate the distance between the thumb tip and middle finger tip
        float distance = Vector3.Distance(thumbTipTransform.position, middleFingerTipTransform.position);

        // Try and grab the object if the distance is low enough, else let the object go if holding something
        if (distance <= minDistance)
            triggerHandler.TryGrab();
        else
            triggerHandler.TryLetGo();
    }
}
