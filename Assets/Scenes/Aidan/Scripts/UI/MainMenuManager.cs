using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu = null;
    public static MainMenuManager Instance;

    public static event Action OnShowEvent;

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

    private void Start()
    {
        // Show the main menu to begin with
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        QuickMenuManager.Instance.DisableMenuAccess();
        QuickMenuManager.Instance.HideMenu();
        if (RayPointer.Instance)
            RayPointer.Instance.EnablePointer();

        OnShowEvent?.Invoke();
    }

    public void HideMainMenu()
    {
        mainMenu.SetActive(false);
    }
}
