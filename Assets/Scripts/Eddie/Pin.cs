using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    float moveSpeed = .05f;
    public Transform destination;
    public Extinguisher extinguisher;
    bool released = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("PIN");
        if (!released)
        {
            extinguisher.isEnabled = true;
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (released)
        {
            float dist = Vector3.Distance(transform.position, destination.position);
            if (dist <= .1f)
            {
                extinguisher.isEnabled = true;
                StartCoroutine(Release());
            }
            transform.position = Vector3.MoveTowards(transform.position, destination.position, moveSpeed * Time.deltaTime);
        }
    }
    IEnumerator Release()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
