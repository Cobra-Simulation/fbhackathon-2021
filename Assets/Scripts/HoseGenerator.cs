using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoseGenerator : MonoBehaviour
{
    public GameObject mainObj;
    public GameObject segmentPrefab, handlePrefab;
    public GameObject nozzleConnector;
    public int numSegments = 25;
    float segmentDist = .75f;
    public HoseSegment[] hoseSegments;
    public float scale = .5f;
    public bool kinematic = true, fakeCol = true, shouldOffset = false;
    public bool extinguisher = true;
    public bool generated = false;
    //public bool kinematic = false;
    private void Start()
    {
        SpawnHose();
    }

    public void DeleteHose()
    {
        for (int i = 0; i < numSegments; i++)
        {
            Destroy(hoseSegments[i].gameObject);
        }
        generated = false;
    }

    public void SpawnHose()
    {
        generated = true;
        Vector3 startPos = nozzleConnector.transform.position;
        hoseSegments = new HoseSegment[numSegments];
        segmentDist = scale + (scale / 2);
        Vector3 diff = Vector3.forward;
        for (int i = 0; i < numSegments; i++)
        {
            GameObject segment = Instantiate(segmentPrefab, transform);
            segment.transform.localScale = new Vector3(scale, scale, scale);

            segment.transform.localPosition = new Vector3(startPos.x + (i * segmentDist), startPos.y, startPos.z);
            hoseSegments[i] = segment.GetComponent<HoseSegment>();
            hoseSegments[i].fakeCol = fakeCol;
            if (i > 0)
            {
                diff = (hoseSegments[i - 1].transform.position - hoseSegments[i].transform.position).normalized;
                hoseSegments[i - 1].transform.rotation = Quaternion.LookRotation(Vector3.down, diff);
                hoseSegments[i - 1].joint.connectedBody = hoseSegments[i].rb;
                hoseSegments[i - 1].joint.autoConfigureConnectedAnchor = false;
                hoseSegments[i - 1].joint.autoConfigureConnectedAnchor = true;
            }
        }

        for (int i = 0; i < numSegments; i++)
        {
            float val = (i % 2 == 0) ? segmentDist : -segmentDist;
            hoseSegments[i].transform.position = new Vector3(transform.position.x + val, transform.position.y, transform.position.z);
        }
        //nozzleConnector.transform.rotation = Quaternion.LookRotation(diff, Vector3.up);
        nozzleConnector.transform.position = hoseSegments[0].transform.position;
        HoseSegment handleSegment = nozzleConnector.GetComponent<HoseSegment>();
        handleSegment.joint.connectedBody = hoseSegments[0].rb;
        handleSegment.joint.autoConfigureConnectedAnchor = false;
        handleSegment.joint.autoConfigureConnectedAnchor = true;
        nozzleConnector.transform.position = transform.position;
        if (kinematic)
        {
            hoseSegments[hoseSegments.Length - 1].rb.isKinematic = true;
        }
        if (shouldOffset && mainObj != null)
        {
            Vector3 offset = nozzleConnector.transform.position - startPos;
            mainObj.transform.position += offset;
        }
    }
}
