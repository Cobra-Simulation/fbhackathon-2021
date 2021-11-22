using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoseUpdate : MonoBehaviour
{
    public Transform connector;

    private void LateUpdate()
    {
        transform.position = connector.transform.position;
        transform.rotation = connector.transform.rotation;
    }

}
