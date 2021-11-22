using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UseOnFryingPanObjective : Objective
{
    public static event Action OnFirePutOutEvent;

    private void OnEnable()
    {
        OnFirePutOutEvent += Complete;
        NoFiresEventSpawner.OnNoActiveFires += Complete;
    }

    private void OnDisable()
    {
        OnFirePutOutEvent -= Complete;
        NoFiresEventSpawner.OnNoActiveFires -= Complete;
    }

    public override bool Tick()
    {
        if (completeEvent)
        {
            OnFirePutOutEvent?.Invoke();
            completeEvent = false;
        }
        return base.Tick();
    }
}
