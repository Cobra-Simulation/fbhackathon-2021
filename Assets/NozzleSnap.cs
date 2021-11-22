using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NozzleSnap : MonoBehaviour
{
    public GrabbableNozzle nozzle;
    bool canSnap = true;
    private void Start()
    {
        StartCoroutine(StartDelay());
    }
    IEnumerator StartDelay()
    {

        yield return new WaitForSeconds(.5f);
        canSnap = false;
        Snap();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canSnap && other.transform.parent.gameObject == nozzle.gameObject)
        {
            canSnap = false;
            nozzle.LetGo();
            Snap();
        }
    }
    
    public void Snap()
    {
        nozzle.canGrab = false;
        nozzle.RB.isKinematic = true;

        nozzle.transform.position = transform.position;
        nozzle.transform.rotation = transform.rotation;
        nozzle.transform.parent = transform;
        nozzle.extinguisher.Disable();
        StartCoroutine(Cooldown(1));
    }
    IEnumerator Cooldown(float time)
    {
        float timer = time;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        nozzle.canGrab = true;
        canSnap = true;
    }
}
