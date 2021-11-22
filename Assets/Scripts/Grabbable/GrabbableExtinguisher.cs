using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GrabbableExtinguisher : Grabbable
{
    public Extinguisher extinguisher;
    public Collider nozzleCol;
    public HoseGenerator hose;
    public override void Enable()
    {
        isEnabled = true;
        RB.useGravity = true;
        if (!hose.generated)
        {
            hose.SpawnHose();
        }
    }

    public override void Disable()
    {
        isEnabled = false;
        RB.useGravity = false;
        if (hose.generated)
        {
            hose.DeleteHose();
        }
    }

    public override void PickUp(ControllerCollisionTrigger _controller)
    {
        if (isEnabled)
        {
            base.PickUp(_controller);

            extinguisher.Grab(controller.gameObject);
            if (nozzleCol != null)
            {
                nozzleCol.enabled = true;
            }
        }
    }
    public override void LetGo()
    {
        if (isEnabled)
        {
            base.LetGo();
            GameObject obj = (controller == null) ? null : controller.gameObject;
            extinguisher.LetGo(obj);
            if (nozzleCol != null)
            {
                nozzleCol.enabled = false;
            }
        }
    }
}
