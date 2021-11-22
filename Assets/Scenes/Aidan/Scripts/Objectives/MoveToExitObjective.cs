using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveToExitObjective : Objective
{
    public static event Action OnDoorOpenedEvent;
    public static event Action OnEnterEvent;

    private void OnEnable()
    {
        OnDoorOpenedEvent += Complete;
        GrabbableDoor.OnDoorOpen += Complete;
    }

    private void OnDisable()
    {
        OnDoorOpenedEvent -= Complete;
        GrabbableDoor.OnDoorOpen -= Complete;
    }

    public override void OnEnter()
    {
        OnEnterEvent?.Invoke();

        base.OnEnter();
    }

    public override bool Tick()
    {
        if (completeEvent)
        {
            OnDoorOpenedEvent?.Invoke();
            completeEvent = false;
        }
        return base.Tick();
    }
}
