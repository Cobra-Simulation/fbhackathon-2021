using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbablePin : Grabbable
{
    public Extinguisher extinguisher;
    Vector3 origin;
    public Transform destination;
    private void Start()
    {
        origin = transform.position;
    }

    public override void ApplyForce(Vector3 force)
    {
        if (isEnabled)
        {
            force = new Vector3(force.x * 0.001f, force.y * 0.001f, force.z * 0.001f);

            RB.AddForce(force);
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, origin.x, destination.position.x),
                                            Mathf.Clamp(transform.position.y, origin.y, destination.position.y),
                                            Mathf.Clamp(transform.position.z, origin.z, destination.position.z));
        }
    }
    public override void Update()
    {
        if (isEnabled && transform.localPosition.z <= destination.localPosition.z)
        {
            extinguisher.Enable();
            Destroy(gameObject);
        }
        base.Update();
    }
}
