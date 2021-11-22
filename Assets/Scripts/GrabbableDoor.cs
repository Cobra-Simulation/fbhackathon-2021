using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GrabbableDoor : Grabbable
{
    public Transform pivot;
    float val = 0;
    public float maxRot = 90, openRot = 30, turnSensitivity = 500f;
    public bool twoWay = true, flipped = false;
    bool open = false;
    public static event Action OnDoorOpen;
    public override void ApplyRotation()
    {
        float dist = transform.InverseTransformPoint(controller.transform.position).x - transform.InverseTransformPoint(transform.position).x;
        dist = (dist * (turnSensitivity * Time.deltaTime));
        if (flipped) dist = -dist;
        val += dist;
        val = Mathf.Clamp(val, 0, (maxRot + 10));
        pivot.localRotation = Quaternion.Euler(0, val, 0);
    }
    
    public override void Update()
    {
        if (!open && pivot.localEulerAngles.y > openRot)
        {
            Debug.Log("OPENED");
            open = true;
            OnDoorOpen?.Invoke();
        }

        if (open && pivot.localEulerAngles.y < openRot)
        {
            Debug.Log("CLOSED");
            open = false;
        }
        
        base.Update();
    }
}
