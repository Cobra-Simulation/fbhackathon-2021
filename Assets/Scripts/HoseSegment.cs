using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoseSegment : MonoBehaviour
{
    public Rigidbody rb;
    public CharacterJoint joint;
    public bool fakeCol = false;
    private void LateUpdate()
    {
        if (fakeCol)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 0, 10000), transform.position.z);
        }
    }
}
