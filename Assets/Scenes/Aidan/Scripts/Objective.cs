using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    [SerializeField] protected GameObject objectiveUIObject = null;
    [SerializeField] protected bool completeEvent = false;
    public bool IsComplete { get; private set; } = false;
    public bool IsActive { get; private set; } = false;

    private void Awake()
    {
        IsActive = false;
        objectiveUIObject.SetActive(false);
    }

    public virtual void OnEnter()
    {
        IsActive = true;
        objectiveUIObject.SetActive(true);
        return;
    }

    public virtual void OnExit()
    {
        IsActive = false;
        objectiveUIObject.SetActive(false);
        return;
    }

    public virtual bool Tick()
    {
        return IsComplete;
    }

    public void Complete()
    {
        if (IsActive)
            IsComplete = true;
    }

    public virtual void ResetObjective()
    {
        IsActive = false;
        IsComplete = false;
        objectiveUIObject.SetActive(false);
        return;
    }
}
