using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GetFireExtinguisherObjective : Objective
{
    [SerializeField] private FireExtinguisherType typeToGet = FireExtinguisherType.WET_CHEMICAL;

    public static event Action<FireExtinguisherType> OnExtinguisherGrabbedEvent;

    private void OnEnable()
    {
        OnExtinguisherGrabbedEvent += CheckFireExtinguisherType;
        Extinguisher.OnGrab += CheckFireExtinguisherType;
    }

    private void OnDisable()
    {
        OnExtinguisherGrabbedEvent -= CheckFireExtinguisherType;
        Extinguisher.OnGrab -= CheckFireExtinguisherType;
    }

    public override bool Tick()
    {
        if (completeEvent)
        {
            OnExtinguisherGrabbedEvent?.Invoke(FireExtinguisherType.WET_CHEMICAL);
            completeEvent = false;
        }
        return base.Tick();
    }

    private void CheckFireExtinguisherType(FireExtinguisherType type)
    {
        // If the correct type has been picked up, complete the objective
        if (type == typeToGet)
            Complete();
    }
}
