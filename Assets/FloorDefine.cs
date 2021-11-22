using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorDefine : MonoBehaviour
{
    public float floorHeight = 0.3f;
    public static float height = 0.2f;


    void Update()
    {
        height = floorHeight;
    }

    public static void FakeFloor(Transform inTransform)
    {
        inTransform.position = new Vector3(inTransform.position.x, Mathf.Clamp(inTransform.position.y, height, 100), inTransform.position.z);
    }
}
