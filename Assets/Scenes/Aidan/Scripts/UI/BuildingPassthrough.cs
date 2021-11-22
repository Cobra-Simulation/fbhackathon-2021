using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPassthrough : MonoBehaviour
{
    [SerializeField] private GameObject model = null;

    private void Awake()
    {
        Hide();
    }

    private void OnEnable()
    {
        MoveToExitObjective.OnEnterEvent += Show;
        MainMenuManager.OnShowEvent += Hide;
    }

    private void OnDisable()
    {
        MoveToExitObjective.OnEnterEvent -= Show;
        MainMenuManager.OnShowEvent -= Hide;
    }

    private void Show()
    {
        model.SetActive(true);
    }

    private void Hide()
    {
        model.SetActive(false);
    }
}
