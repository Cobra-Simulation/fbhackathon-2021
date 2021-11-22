using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCollisionTrigger : MonoBehaviour
{
    public static float pullForce = 100.0f;
    [SerializeField] private Material triggerMat = null;
    [SerializeField] private bool showDebugSphere = false;
    [SerializeField] private GameObject controllerModelObject = null;
    private MeshRenderer meshRenderer = null;
    private Grabbable currentGrabbable = null;
    private Material originalMat = null;
    private bool isHoldingSomething = false;
    private Transform originalParent = null;
    private bool moveGrabbableTowardsController = false;

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
        if (currentGrabbable != null && other.gameObject == currentGrabbable.gameObject)
            currentGrabbable = null;
    }

    public void TryGrab()
    {
        if (isHoldingSomething) return;
        Grab();
    }

    public void TryLetGo()
    {
        if (!isHoldingSomething) return;
        LetGo();
    }

    private void Grab()
    {
        isHoldingSomething = true;

        currentGrabbable.PickUp(this);

        // Hide the controller model
        controllerModelObject.SetActive(false);

        // This changes the colour of the debug sphere when grabbing an object
        meshRenderer.material = triggerMat;
    }

    private void LetGo()
    {
        isHoldingSomething = false;

        // This changes the colour of the debug sphere back to it's original material
        meshRenderer.material = originalMat;

        // Show the controller model
        controllerModelObject.SetActive(true);
        currentGrabbable.LetGo();
        // Switch how the grabbable letting go
        // it handled based on the type

    }
}
