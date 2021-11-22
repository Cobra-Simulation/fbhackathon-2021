using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Point
{
    public Vector3 pos, prevPos;
    public bool locked;
}
public class Stick
{
    public Point pointA, pointB;
    public Vector3 center;
    public float length;
}
public class HosePipe : MonoBehaviour
{
    public GameObject hosePrefab, spherePrefab;
    public float gravity = 1, scale = .25f;
    public int numSegments = 10, maxIter = 100;
    const float minAcceptableDist = 0.01f;
    Transform[] pointObjects;
    public Transform target, basePos;

    void Start()
    {
        GeneratePipe();
    }
    void Update()
    {
        Solve();
    }
    public void GeneratePipe()
    {
        if (numSegments % 2 != 0)
        {
            numSegments++;
        }
        pointObjects = new Transform[numSegments];
        Vector3 dir = (target.position - transform.position).normalized;
        for (int i = 0; i < numSegments; i++)
        {
            GameObject hoseSegment = null;
            if (i % 2 == 0 || i == numSegments - 1)
            {
                hoseSegment = Instantiate(spherePrefab, transform);
            }
            else
            {
                hoseSegment = Instantiate(hosePrefab, transform);
            }
            pointObjects[i] = hoseSegment.transform;
            pointObjects[i].position = transform.position + (dir * (i * scale));
        }
    }
    public void Solve()
    {
        if (Vector3.Distance(target.position, pointObjects[pointObjects.Length - 1].position) > .1)
        {
            target.position = Vector3.MoveTowards(target.position, pointObjects[pointObjects.Length - 1].position, 100 * Time.deltaTime);
        }
        Vector3[] points = new Vector3[pointObjects.Length];
        Vector3[] dirs = new Vector3[pointObjects.Length];
        for (int i = 0; i < pointObjects.Length; i++)
        {
            points[i] = pointObjects[i].position;
        }
        Vector3 origin = points[0];

        float[] segmentLengths = new float[points.Length - 1];
        for (int i = 0; i < points.Length - 1; i++)
        {
            segmentLengths[i] = (points[i + 1] - points[i]).magnitude;
        }

        for (int i = 0; i < maxIter; i++)
        {
            bool startingFromTarget = i % 2 == 0;

            System.Array.Reverse(points);
            System.Array.Reverse(segmentLengths);
            points[0] = (startingFromTarget) ? target.position : origin;

            for (int j = 1; j < points.Length; j++)
            {
                dirs[j] = (points[j] - points[j - 1]).normalized;
                points[j] = points[j - 1] + dirs[j] * segmentLengths[j - 1];
            }

            float dstToTarget = (points[points.Length - 1] - target.position).magnitude;
            if (!startingFromTarget && dstToTarget <= minAcceptableDist)
            {
                break;
            }
        }
        for (int i = 0; i < pointObjects.Length; i++)
        {
            pointObjects[i].position = points[i];
            if (i + 1 < pointObjects.Length)
            {
                pointObjects[i].LookAt(points[i + 1]);
            }
            else
            {
                pointObjects[i].LookAt(target);
            }
            pointObjects[i].Rotate(90, 0, 0);
        }
        return;
    }
    /*
    void Simulate(Vector3[] positions)
    {
        for (int i = 0; i < points.Length; i++)
        {

            if (points[i] != null && !points[i].locked)
            {
                Vector3 posBeforeUpdate = points[i].pos;
                if (points[i].pos.z > 0)
                {
                    points[i].pos += points[i].pos - points[i].prevPos;
                    points[i].pos += Vector3.down * gravity * Time.deltaTime * Time.deltaTime;
                }
                points[i].prevPos = posBeforeUpdate;
            }
        }

        for (int i = 0; i < maxIter; i++)
        {
            for (int j = 0; j < sticks.Length; j++)
            {

                if (sticks[j] != null)
                {
                    sticks[j].center = (sticks[j].pointA.pos + sticks[j].pointB.pos) / 2;
                    Vector3 stickDir = (sticks[j].pointA.pos - sticks[j].pointB.pos).normalized;
                    if (!sticks[j].pointA.locked)
                    {
                        sticks[j].pointA.pos = sticks[j].center + stickDir * sticks[j].length / 2;
                    }
                    if (!sticks[j].pointB.locked)
                    {
                        sticks[j].pointB.pos = sticks[j].center - stickDir * sticks[j].length / 2;
                    }
                }
            }
        }

        for (int i = 0; i < points.Length; i++)
        {
            points[i].transform.position = points[i].pos;
        }
        for (int i = 0; i < sticks.Length; i++)
        {
            sticks[i].transform.position = sticks[i].center;
            sticks[i].transform.localScale = new Vector3(sticks[i].transform.localScale.x, sticks[i].length, sticks[i].transform.localScale.z);
        }
    }
    */
}