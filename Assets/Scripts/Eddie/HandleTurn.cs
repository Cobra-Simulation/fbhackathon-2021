using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleTurn : MonoBehaviour
{
    public WaterSpray spray;
    float min = 0, max = 45;
    public Transform pivot;
    public void ApplyForce(float val)
    {
        val = Mathf.Clamp(val, 0, 45);
        spray.UpdatePressure(val);
        if ((val < 0 && transform.localRotation.eulerAngles.z > min) || (val > 0 && transform.localRotation.eulerAngles.z < max))
        {
            transform.Rotate(new Vector3(0, 0, val));
            spray.UpdatePressure(transform.localRotation.eulerAngles.z);
        }
    }
}
