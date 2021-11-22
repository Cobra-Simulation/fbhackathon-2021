using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtinguisherStand : MonoBehaviour
{
    public float snapTime = .5f, snapSpeed = 10;
    public Extinguisher extinguisher1, extinguisher2;
    public Vector3 basePos1, basePos2;
    Coroutine snapping1, snapping2;
    // Start is called before the first frame update
    void Start()
    {
        basePos1 = extinguisher1.transform.position;
        basePos2 = extinguisher2.transform.position;
    }
    private void Update()
    {
        FloorDefine.FakeFloor(transform);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == extinguisher1.gameObject)
        {
            snapping1 = StartCoroutine(SnapTimer(extinguisher1, basePos1));
        }
        if (other.gameObject == extinguisher2.gameObject)
        {
            snapping2 = StartCoroutine(SnapTimer(extinguisher2, basePos2));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == extinguisher1.gameObject && snapping1 != null)
        {
            StopCoroutine(snapping1);
            snapping1 = null;
        }
        if (other.gameObject == extinguisher2.gameObject && snapping2 != null)
        {
            StopCoroutine(snapping2);
            snapping2 = null;
        }
    }
    IEnumerator SnapTimer(Extinguisher _extinguisher, Vector3 pos)
    {
        float timer = 0.5f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        _extinguisher.grab.LetGo();
        _extinguisher.rb.isKinematic = true;
        _extinguisher.transform.position = pos;
    }
}
