using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTriggerHandler : MonoBehaviour
{
    [SerializeField] private Transform hand = null;
    [SerializeField] private Material gestureMat = null;
    [SerializeField] private bool showDebugSphere = true;
    [SerializeField] private float pullForce = 100.0f;
    private Grabbable currentGrabbable = null;
    private bool isHoldingSomething = false;
    private Transform originalParent = null;
    private MeshRenderer meshRenderer = null;
    private Material originalMat = null;
    private bool moveGrabbableTowardsHand = false;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = showDebugSphere;
        originalMat = meshRenderer.material;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Return if holding something already
        if (isHoldingSomething) return;

        // If the other collider has a grabbable component, set it to the current object to grab
        Grabbable grabbable = other.GetComponent<Grabbable>();
        if (grabbable)
        {
            // Compare the distance between the new grabbable and the current to find the smallest
            if (currentGrabbable != null)
            {
                float distToNewGrabbable = Vector3.Distance(grabbable.transform.position, transform.position);
                float distToOldGrabbable = Vector3.Distance(currentGrabbable.transform.position, transform.position);
                currentGrabbable = distToNewGrabbable < distToOldGrabbable ? grabbable : currentGrabbable;
            }
            else
                currentGrabbable = grabbable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Return if holding something already
        if (isHoldingSomething) return;

        // If the object has left the collider, the player shouldn't be able to grab it
        if (other.gameObject == currentGrabbable.gameObject)
            currentGrabbable = null;
    }

    public void TryGrab()
    {
        // Return if holding something already
        if (isHoldingSomething) return;

        Grab();
    }

    public void TryLetGo()
    {
        // Return if not holding something
        if (!isHoldingSomething) return;

        LetGo();
    }

    private void Grab()
    {
        meshRenderer.material = gestureMat;

        isHoldingSomething = true;

        // Change how the grab is handled depending on the type of grabbable
        switch (currentGrabbable.Type)
        {
            case Grabbable.GrabbableType.PICKUP:
                HandleGrabPickup();
                break;
            case Grabbable.GrabbableType.MOVE:
                HandleGrabMovable();
                break;
            default:
                break;
        }
    }

    private void LetGo()
    {
        meshRenderer.material = originalMat;

        // Set the object's parent back to what it was initially
        isHoldingSomething = false;

        // Switch how the grabbable letting go it handled based on the type
        switch (currentGrabbable.Type)
        {
            case Grabbable.GrabbableType.PICKUP:
                HandleLetGoPickup();
                break;
            case Grabbable.GrabbableType.MOVE:
                HandleLetGoMoveable();
                break;
            default:
                break;
        }
    }

    private void HandleGrabPickup()
    {
        // Store the object's parent and set it's new parent to the hand
        // If the parent of the object is a hand, don't update the original parent
        if (currentGrabbable.transform.parent.tag != "Hand")
            originalParent = currentGrabbable.transform.parent;
        currentGrabbable.transform.parent = hand;

        // Disable the object's rigidbody if it has one
        Rigidbody gRB = currentGrabbable.RB;
        if (gRB)
        {
            gRB.isKinematic = true;
        }
    }

    private void HandleLetGoPickup()
    {
        // Return if the other hand is holding this object
        if (currentGrabbable.transform.parent.tag == "Hand" && currentGrabbable.transform.parent != hand)
            return;

        currentGrabbable.transform.parent = originalParent;

        // Enable the object's rigidbody if it has one
        Rigidbody gRB = currentGrabbable.RB;
        if (gRB)
        {
            gRB.isKinematic = false;
        }
    }

    private void HandleGrabMovable()
    {
        // Get the object's rigidbody and apply a force from it towards the position on the hand
        moveGrabbableTowardsHand = true;
        Rigidbody rb = currentGrabbable.RB;
        if (rb)
            StartCoroutine(MoveTowardsHand(rb));
    }

    private void HandleLetGoMoveable()
    {
        // Stop applying the force to the grabbable object
        moveGrabbableTowardsHand = false;
    }

    private IEnumerator MoveTowardsHand(Rigidbody rb)
    {
        // Keep adding the pull force to the grabbable object while moveGrabbableTowardsHand is true
        while (moveGrabbableTowardsHand)
        {
            Vector3 dirToController = hand.transform.position - currentGrabbable.transform.position;
            rb.AddForce(dirToController * pullForce);
            yield return null;
        }
    }

}
