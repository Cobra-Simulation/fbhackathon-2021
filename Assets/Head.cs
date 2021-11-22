using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Head : MonoBehaviour
{
    public float crawlThreshold = 1;
    public bool crawling = false, triggered = false;

    public static event Action OnCrawl;

    public AudioSource audio;
    public void ActivateAlarm()
    {
        audio.Play();
    }
    void Update()
    {
        if (transform.position.y <= crawlThreshold)
        {
            crawling = true;
            if (!triggered)
            {
                triggered = true;
                OnCrawl?.Invoke();
            }
        }
        else
        {
            crawling = false;
            triggered = false;
        }
    }
}
