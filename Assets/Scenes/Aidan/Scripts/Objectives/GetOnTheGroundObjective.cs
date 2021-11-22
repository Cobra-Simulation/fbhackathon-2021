using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GetOnTheGroundObjective : Objective
{
    public static event Action OnPlayerOnGroundEvent;
    public static event Action OnEnterEvent;

    private void OnEnable()
    {
        OnPlayerOnGroundEvent += Complete;
        Head.OnCrawl += Complete;
    }

    private void OnDisable()
    {
        OnPlayerOnGroundEvent -= Complete;
        Head.OnCrawl -= Complete;
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
            OnPlayerOnGroundEvent?.Invoke();
            completeEvent = false;
        }
        return base.Tick();
    }
}
