using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableWindow : Grabbable
{
    public Window window;
    Vector3 origin;
    public Transform destination;
    private void Start()
    {
        origin = transform.position;
    }
    public override void ApplyForce(Vector3 force)
    {
        if (!window.open)
        {
            force = new Vector3(0, force.y, 0);
            RB.AddForce(force);
        }
    }
    public override void Update()
    {
        base.Update();
        if (!isEnabled)
        {
            origin = transform.position;
        }
    }

    public override void Enable()
    {
        base.Enable();
        RB.isKinematic = false;
        origin = transform.position;
    }
    public override void Disable()
    {
        base.Disable();
        RB.isKinematic = true;
    }
    private void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x,
                        Mathf.Clamp(transform.position.y, origin.y, destination.position.y),
                        transform.position.z);
        if (isEnabled && !window.open)
        {
            if (transform.position.y >= destination.position.y)
            {
                window.Open();
                Destroy(RB);
            }
        }
    }
}
