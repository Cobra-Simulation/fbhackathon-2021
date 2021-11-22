using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;
using UnityEngine.SceneManagement;

public class ControllerInput : MonoBehaviour
{
    public enum Controller
    {
        LEFT,
        RIGHT
    }
    [SerializeField] public Controller controller = Controller.LEFT;

    private ControllerCollisionTrigger collisionTrigger;

    private void Awake()
    {
        collisionTrigger = GetComponent<ControllerCollisionTrigger>();
    }

    private void Update()
    {
        switch (controller)
        {
            case Controller.LEFT:
                HandleLeftControllerInput();
                break;
            case Controller.RIGHT:
                HandleRightControllerInput();
                break;
            default:
                break;
        }

        if (!LevelEditorManager.Instance.IsActive)
        {
            // Go back to the main menu if the player presses the B button
            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                MainMenuManager.Instance.ShowMainMenu();
            }
        }
    }

    private void HandleLeftControllerInput()
    {
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0.1)
            collisionTrigger.TryGrab();
        else
            collisionTrigger.TryLetGo();
    }

    private void HandleRightControllerInput()
    {
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.1)
            collisionTrigger.TryGrab();
        else
            collisionTrigger.TryLetGo();
    }
}
