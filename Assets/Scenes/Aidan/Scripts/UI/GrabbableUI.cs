using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableUI : Grabbable
{
    public override void Enable()
    {
        isEnabled = true;
    }

    public override void Disable()
    {
        isEnabled = false;
    }

    public override void PickUp(ControllerCollisionTrigger _controller)
    {
        if (!canGrab || Type != GrabbableType.PICKUP) return;

        controller = _controller;
        transform.parent = controller.transform;
        grabbed = true;
    }

    public override void LetGo()
    {
        transform.parent = startParent;
        grabbed = false;
        controller = null;
    }
}
