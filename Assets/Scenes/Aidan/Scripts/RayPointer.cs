using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RayPointer : MonoBehaviour
{
    [SerializeField] private float rayLength = 100.0f;
    [SerializeField] private GameObject eventSystemPrefab = null;
    private GameObject eventSystemObj = null;
    private EventSystem eventSystem = null;
    private StandaloneInputModule inputModule = null;
    private LineRenderer lineRenderer = null;
    public bool IsEnabled { get; private set; }

    public static RayPointer Instance;

    private void Awake()
    {
        // Singleton pattern
        if (Instance)
        {
            if (Instance != this)
            {
                Destroy(this);
            }
        }
        else
        {
            Instance = this;
        }

        // If there isn't an event system with a custom input module in the scene, create one
        CustomInputModule customInputModule = FindObjectOfType<CustomInputModule>();
        if (customInputModule)
            eventSystemObj = FindObjectOfType<CustomInputModule>().gameObject;
        if (!eventSystemObj)
        {
            eventSystemObj = Instantiate<GameObject>(eventSystemPrefab, null);
            customInputModule = eventSystemObj.GetComponent<CustomInputModule>();
        }
        customInputModule.EventCamera = GetComponent<Camera>();
        customInputModule.ClickButton = OVRInput.Button.PrimaryIndexTrigger;
        customInputModule.Controller = OVRInput.Controller.RTouch;
        eventSystem = eventSystemObj.GetComponent<EventSystem>();
        inputModule = eventSystemObj.GetComponent<StandaloneInputModule>();

        // Cache components
        lineRenderer = GetComponent<LineRenderer>();

    }

    private void Update()
    {
        // Update the positions in the line renderer
        if (lineRenderer.enabled)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, GetEnd());
        }

        // If the quick menu is not accessable, the start button will still turn the ray on
        if (!QuickMenuManager.Instance.IsMenuAccessable)
        {
            if (OVRInput.GetDown(OVRInput.Button.Start))
            {
                if (IsEnabled)
                    DisablePointer();
                else
                    EnablePointer();
            }
        }
    }

    private Vector3 GetEnd()
    {
        float distance = GetCanvasDistance();
        Vector3 endPosition = CalculateEnd(rayLength);
        if (distance > 0.0f)
        {
            endPosition = CalculateEnd(distance);
        }

        return endPosition;
    }

    private Vector3 CalculateEnd(float rayLength)
    {
        return transform.position + transform.forward * rayLength;
    }

    private float GetCanvasDistance()
    {
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = inputModule.inputOverride.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(eventData, results);

        RaycastResult closestResult = FindFirstRaycast(results);
        float distance = closestResult.distance;

        distance = Mathf.Clamp(distance, 0, rayLength);
        return distance;
    }

    private RaycastResult FindFirstRaycast(List<RaycastResult> results)
    {
        foreach(RaycastResult result in results)
        {
            if (!result.gameObject)
                continue;
            return result;
        }
        return new RaycastResult();
    }

    public void DisablePointer()
    {
        IsEnabled = false;
        lineRenderer.enabled = false;
    }

    public void EnablePointer()
    {
        IsEnabled = true;
        lineRenderer.enabled = true;
    }
}
