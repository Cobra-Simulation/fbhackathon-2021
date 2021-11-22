using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void NoActiveFiresEventHandler();

public class NoFiresEventSpawner : MonoBehaviour
{
    public NoFiresEventSpawner()
    {
        LevelEditorManager.OnEnabledEvent += SetLevelEditorOpen;
        LevelEditorManager.OnDisabledEvent += SetLevelEditorClose;
    }

    private bool isEventEditorOpen = false;
    private bool hasFireExisted = false;
    private bool hasEventBeenFireBeforeEditorReset = false;
   
    public static event NoActiveFiresEventHandler OnNoActiveFires;


    private void Update()
    {
        if (!hasFireExisted)
        {
            if (FireManager.Instance.ActiveFires > 0)
            {
                hasFireExisted = true;
            }
        }

        // If no fires left, event hasen't been sent yet, a fire has existed and the editor isnt open
        if (FireManager.Instance.ActiveFires <= 0 && hasEventBeenFireBeforeEditorReset == false && hasFireExisted == true && !isEventEditorOpen)
        {
            // No active fires present
            OnNoActiveFires();
            hasEventBeenFireBeforeEditorReset = true;
        }
    }

    private void SetLevelEditorOpen()
    {
        isEventEditorOpen = true;
        //hasEventBeenFireBeforeEditorReset = true;
    }

    private void SetLevelEditorClose()
    {
        isEventEditorOpen = false;
        //hasEventBeenFireBeforeEditorReset = false;
    }
}
