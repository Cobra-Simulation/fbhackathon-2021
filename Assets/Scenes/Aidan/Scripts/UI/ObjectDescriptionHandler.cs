using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDescriptionHandler : MonoBehaviour
{
    [SerializeField] private GameObject descriptionObject = null;
    private Camera camera = null;

    private void Awake()
    {
        HideDescriptionBox();
        camera = Camera.main;
    }

    private void Update()
    {
        if (RayPointer.Instance.IsEnabled)
            CheckForRayPointerIntersection();
        else
            HideDescriptionBox();

        // Rotate the description box to point towards the camera
        Vector3 camForwardVec = camera.transform.forward;
        Vector3 camUpVec = camera.transform.up;
        Quaternion newRotation = Quaternion.LookRotation(camForwardVec, Vector3.up);
        //newRotation.SetAxisAngle(Vector,);
        descriptionObject.transform.rotation = newRotation;
    }

    public void ShowDescriptionBox()
    {
        descriptionObject.SetActive(true);
    }

    public void HideDescriptionBox()
    {
        descriptionObject.SetActive(false);
    }

    private void CheckForRayPointerIntersection()
    {
        // Try and find the current object out of all the objects hit by a ray cast from the controller
        Ray ray = new Ray(RayPointer.Instance.transform.position, RayPointer.Instance.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100.0f);
        bool found = false;
        for (int i = 0; i < hits.Length; i++)
        {
            found = hits[i].collider.gameObject == gameObject;
            if (found) break;
        }
        if (found)
            ShowDescriptionBox();
        else
            HideDescriptionBox();
    }
}
