using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class ScenarioOptionsMenu : MonoBehaviour
{
    [SerializeField] private int scenario1ObjectiveIndex = 0;
    [SerializeField] private int scenario2ObjectiveIndex = 2;
    [SerializeField] private int scenario3ObjectiveIndex = 4;

    public static event Action OnScenario1SelectEvent;
    public static event Action OnScenario2SelectEvent;
    public static event Action OnScenario3SelectEvent;
    public static event Action OnScenarioStartedEvent;

    public void OnScenario1Select()
    {
        OnScenarioChange();
        ObjectiveManager.Instance.ChangeStartIndex(scenario1ObjectiveIndex);
        OnScenario1SelectEvent?.Invoke();
    }

    public void OnScenario2Select()
    {
        OnScenarioChange();
        ObjectiveManager.Instance.ChangeStartIndex(scenario2ObjectiveIndex);
        OnScenario2SelectEvent?.Invoke();
    }

    public void OnScenario3Select()
    { 
        OnScenarioChange();
        ObjectiveManager.Instance.ChangeStartIndex(scenario3ObjectiveIndex);
        OnScenario3SelectEvent?.Invoke();
    }

    private void OnScenarioChange()
    {
        MainMenuManager.Instance.HideMainMenu();
        QuickMenuManager.Instance.DisableMenuAccess();
        RayPointer.Instance.DisablePointer();
        OnScenarioStartedEvent?.Invoke();
    }
}
