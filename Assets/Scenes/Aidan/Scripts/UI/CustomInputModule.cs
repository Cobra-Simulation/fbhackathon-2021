using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomInputModule : BaseInput
{
    [SerializeField] private Camera eventCamera = null;
    [SerializeField] private OVRInput.Button clickButton = OVRInput.Button.PrimaryIndexTrigger;
    [SerializeField] private OVRInput.Controller controller = OVRInput.Controller.All;
    public Camera EventCamera { get { return eventCamera; } set { eventCamera = value; } }
    public OVRInput.Button ClickButton { get { return clickButton; } set { clickButton = value; } }
    public OVRInput.Controller Controller { get { return controller; } set { controller = value; } }

    protected override void Awake()
    {
        GetComponent<BaseInputModule>().inputOverride = this;
    }

    public override bool GetMouseButton(int button)
    {
        return OVRInput.Get(clickButton, controller);
    }

    public override bool GetMouseButtonDown(int button)
    {
        return OVRInput.GetDown(clickButton, controller);
    }

    public override bool GetMouseButtonUp(int button)
    {
        return OVRInput.GetUp(clickButton, controller);
    }

    public override Vector2 mousePosition
    {
        get 
        {
            return new Vector2(eventCamera.pixelWidth / 2, eventCamera.pixelHeight / 2);
        }
    }

    public override bool mousePresent
    {
        get 
        {
            if (OVRInput.GetActiveController() != OVRInput.Controller.None)
                return true;
            else
                return false;
        }
    }
}
