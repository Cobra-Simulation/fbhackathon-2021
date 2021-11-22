using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Window : MonoBehaviour
{
    public bool shouted = false, open = false, listening = false;
    public Transform endPoint;
    float openSpeed = 10;
    public Oculus.Voice.AppVoiceExperience voiceExperience;

    public static event Action OnShout, OnWindowOpen;
    public void Shout()
    {
        Debug.Log("SHOUTED");
        OnShout?.Invoke();
        listening = false;
        Head head = FindObjectOfType<Head>();
        head.ActivateAlarm();
    }

    public void Open()
    {
        Debug.Log("OPEN");
        open = true;
        OnWindowOpen?.Invoke();
        StartCoroutine(KeepListening());
    }
    
    IEnumerator KeepListening()
    {
        voiceExperience.Activate();

        float time = 20;
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        if (listening)
        {
            StartCoroutine(KeepListening());
        }
    }
}
