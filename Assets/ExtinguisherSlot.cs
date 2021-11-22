using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtinguisherSlot : MonoBehaviour
{
    public Extinguisher extinguisher;
    bool canSnap = true;
    float cooldownTime = .5f;
    public AudioSource audio;

    private void OnTriggerExit(Collider other)
    {
        if (canSnap)
        {
            Debug.Log("EXIT");
            if (extinguisher != null && other.gameObject == extinguisher.gameObject)
            {
                canSnap = false;
                extinguisher.slot = null;
                extinguisher.ResetParent();
                extinguisher = null;
            }
            StartCoroutine(Cooldown(1.5f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canSnap && extinguisher == null)
        {
            Extinguisher thisExtinguisher = other.GetComponent<Extinguisher>();
            if (thisExtinguisher != null)
            {
                audio.pitch = Random.Range(.95f, 1.05f);
                audio.Play();
                canSnap = false;
                thisExtinguisher.slot = this;
                thisExtinguisher.DisablePickup();
                thisExtinguisher.transform.parent = transform;
                thisExtinguisher.transform.position = transform.position;
                thisExtinguisher.transform.rotation = transform.rotation;
                extinguisher = thisExtinguisher;
                StartCoroutine(Cooldown(.5f));
            }
        }
    }
    IEnumerator Cooldown(float time)
    {
        float timer = time;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        canSnap = true;
    }
}
