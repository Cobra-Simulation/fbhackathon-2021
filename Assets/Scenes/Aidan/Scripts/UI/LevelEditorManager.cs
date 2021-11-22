using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class LevelEditorManager : MonoBehaviour
{
    public static LevelEditorManager Instance;
    public bool IsActive { get; private set; }

    public static event Action OnEnabledEvent;
    public static event Action OnDisabledEvent;

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
    }

    private void Update()
    {
        if (!IsActive) return;

        // If the player presses B they will exit level editor mode
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            DisableLevelEditorMode();
        }
    }

    public void RestartLevel()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void EnableLevelEditorMode()
    {
        IsActive = true;

        // Hide the main menu and make the quick menu accessible and enabled
        MainMenuManager.Instance.HideMainMenu();
        QuickMenuManager.Instance.EnableMenuAccess();
        QuickMenuManager.Instance.ShowMenu();

        OnEnabledEvent?.Invoke();
    }

    public void DisableLevelEditorMode()
    {
        IsActive = false;

        // Show the main menu again
        MainMenuManager.Instance.ShowMainMenu();
        QuickMenuManager.Instance.HideMenu();
        QuickMenuManager.Instance.DisableMenuAccess();        
        OnDisabledEvent?.Invoke();
    }
}
