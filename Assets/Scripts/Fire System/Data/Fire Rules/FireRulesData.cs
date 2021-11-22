using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FireTypes;

[CreateAssetMenu(fileName = "FireRule", menuName = "Fire System/Fire Rule", order = 1)]
public class FireRulesData : ScriptableObject
{
    [Header("Fire Rule Name")]
    public string Name;

    [Header("Fire Configs")]
    [Tooltip("The type of spray to be used.")]
    public Spray.Type sprayType;
    [Tooltip("The types of fire sources the spray type can be used on.")]
    public FireSource[] fireSources;    

    [Header("Fire Stats")]
    [Tooltip("The grow rate applied when the wrong spray is used.")]
    public float wrongSprayGrowRate;
    [Tooltip("The rate at which the fire grows each tick.")]
    public float fireGrowRate;
    [Tooltip("The health of the fire when it first spawns")]
    public float fireStartHealth;
    [Tooltip("The max health of the fire after growing.")]
    public float fireMaxHealth;
}
