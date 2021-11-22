using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableNozzle : Grabbable
{
    public Transform snapPoint;
    public Extinguisher extinguisher;

    public override void Enable()
    {
        isEnabled = true;
        if (transform.parent == null || transform.parent == snapPoint)
        {
            RB.isKinematic = false;
        }
    }

    public override void Disable()
    {
        isEnabled = false;
        RB.isKinematic = true;
    }

    //private void Update()
    //{
    //    if (enabled && !grabbed && transform.position != snapPoint.position)
    //    {
    //        Snap();
    //    }
    //}

    //public void Snap()
    //{
    //    RB.isKinematic = true;
    //    transform.position = snapPoint.position;
    //    transform.rotation = snapPoint.rotation;
    //    transform.parent = snapPoint.transform;
    //    extinguisher.Disable();
    //}
}
