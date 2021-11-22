using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FireTypes;


[CreateAssetMenu(fileName = "FireAsset", menuName = "Fire System/Combustable Data", order = 1)]
public class CombustibleData : ScriptableObject
{
    public string Name;
    public FireSource Source;
    public Color MarkerColor;
}
