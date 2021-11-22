using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickMenuManager : MonoBehaviour
{
    public static QuickMenuManager Instance;

    private GameObject quickMenu = null;
    private bool isMenuActive = false;
    public bool IsMenuAccessable { get; private set; } = true;
    public GameObject levelToolTips;

    public bool IsMenuActive
    {
        get { return isMenuActive; }
    }

    private void Awake()
    {
        // Singleton pattern
        if (Instance)
        {
            if (Instance != this)
                Destroy(this);
        }
        else
        {
            Instance = this;
        }

        // This object should only have one child which is the quick menu
        quickMenu = transform.GetChild(0).gameObject;
        levelToolTips.SetActive(false);
    }

    private void Start()
    {
        // Turn it off by default
        HideMenu();
    }

    private void Update()
    {
        if (!IsMenuAccessable) return;

        // If the player has pressed the menu button, the menu will toggle on or off
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            if (isMenuActive)
                HideMenu();
            else
                ShowMenu();
        }
    }

    public void ShowMenu()
    {
        isMenuActive = true;
        quickMenu.SetActive(true);
        RayPointer.Instance.EnablePointer();
        levelToolTips.SetActive(true);
    }

    public void HideMenu()
    {
        isMenuActive = false;
        quickMenu.SetActive(false);
        RayPointer.Instance.DisablePointer();
        levelToolTips.SetActive(false);
    }

    public void EnableMenuAccess() => IsMenuAccessable = true;

    public void DisableMenuAccess() => IsMenuAccessable = false;
}
