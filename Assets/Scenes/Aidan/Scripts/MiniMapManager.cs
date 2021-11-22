using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    [SerializeField] private GameObject cobraHQHolder = null;
    [SerializeField] private float minScale = 1.0f;
    [SerializeField] private float maxScale = 2.0f;
    [SerializeField] private float scaleSpeed = 2.0f;

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
        MainMenuManager.OnShowEvent += Hide;
    }

    private void Update()
    {
        // Decrease or increase the size depending on the input
        float inputVal = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y;
        float currentScaleVal = transform.localScale.x + inputVal * (scaleSpeed * Time.deltaTime);
        currentScaleVal = Mathf.Clamp(currentScaleVal, minScale, maxScale);
        transform.localScale = new Vector3(currentScaleVal, currentScaleVal, currentScaleVal);
    }

    private void Show()
    {
        cobraHQHolder.SetActive(true);
    }

    private void Hide()
    {
        cobraHQHolder.SetActive(false);
    }
}
