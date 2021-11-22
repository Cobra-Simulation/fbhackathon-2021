using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float maxRot = 110;
    public bool openIn = true;
    private void LateUpdate()
    {
        transform.localRotation = Quaternion.Euler(0, Mathf.Clamp(transform.localRotation.eulerAngles.y, 0, (openIn) ? maxRot : -maxRot), 0);
    }
}
