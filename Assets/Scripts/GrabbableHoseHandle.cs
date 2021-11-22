using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableHoseHandle : Grabbable
{
    public Transform pivot;
    public WaterSpray spray;
    float val = 0, turnSensitivity = 1500f;
    public override void ApplyRotation()
    {
        if (enabled)
        {
            float dist = transform.InverseTransformPoint(controller.transform.position).x - transform.InverseTransformPoint(transform.position).x;
            dist = -(dist * (turnSensitivity * Time.deltaTime));
            val += dist;
            val = Mathf.Clamp(val, 0, 45);
            pivot.localRotation = Quaternion.Euler(0, 0, val);
            spray.UpdatePressure(val);
        }
    }
}
