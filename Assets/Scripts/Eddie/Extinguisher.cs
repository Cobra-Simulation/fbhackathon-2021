using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Extinguisher : MonoBehaviour
{
    public Spray spray;
    public bool isEnabled = false;
    ControllerInput grabbedHand;
    public Grabbable grab;
    Quaternion startRot;
    public AudioSource audio;
    public FireExtinguisherType type;
    public Rigidbody rb;
    public ExtinguisherSlot slot;
    Transform thisParent;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<Grabbable>();
        startRot = transform.rotation;
        thisParent = transform.parent;
    }
    public void ResetParent()
    {
        transform.parent = thisParent;
    }
    public void Enable()
    {
        isEnabled = true;
    }
    public static event Action<FireExtinguisherType> OnGrab;
    public void Grab(GameObject hand)
    {
        StopAllCoroutines();
        grabbedHand = hand.GetComponent<ControllerInput>();
        OnGrab?.Invoke(type);
        if (slot != null)
        {
            slot = null;
        }
    }

    public void LetGo(GameObject hand)
    {
        if (hand == null || grabbedHand.gameObject == hand)
        {
            grabbedHand = null;
        }
        spray.power = 0;
        audio.volume = 0;
    }

    public void DisablePickup()
    {
        grab.canGrab = false;
        grab.LetGo();
        rb.isKinematic = true;
        StartCoroutine(PickupCooldown());
    }
    IEnumerator PickupCooldown()
    {
        float timer = 1;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        grab.canGrab = true;
    }




    public void Disable()
    {
        spray.power = 0;
        audio.volume = 0;
    }

    void Update()
    {
        FloorDefine.FakeFloor(transform);
        if (isEnabled && grabbedHand != null)
        {
            HandlePressure();
        }
    }
    void HandlePressure()
    {
        if (grabbedHand.controller == ControllerInput.Controller.LEFT)
        {
            float val = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
            spray.power = val * 100;
            audio.volume = Mathf.Lerp(0, 1, val);
        }
        else
        {
            float val = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
            spray.power = val * 100;
            audio.volume = Mathf.Lerp(0, 1, val);
        }
    }
}
