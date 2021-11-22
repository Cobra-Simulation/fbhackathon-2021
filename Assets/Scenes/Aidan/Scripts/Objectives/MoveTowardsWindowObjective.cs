using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveTowardsWindowObjective : Objective
{
    public static event Action OnShoutFireEvent;

    private void OnEnable()
    {
        OnShoutFireEvent += Complete;
        Window.OnShout += Complete;
    }

    private void OnDisable()
    {
        OnShoutFireEvent -= Complete;
        Window.OnShout -= Complete;
    }

    public override bool Tick()
    {
        if (completeEvent)
        {
            OnShoutFireEvent?.Invoke();
            completeEvent = false;
        }
        return base.Tick();
    }
}
