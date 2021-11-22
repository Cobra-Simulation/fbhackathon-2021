using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR;

public class SmokeEventSpawner : MonoBehaviour
{
    private bool smokeEnabled = false;
    public GameObject smokePrefab;
    public Camera playerPosition;

    private GameObject activeSmoke = null;

    public SmokeEventSpawner()
    {
        // Create a event for the objectives manager
        GetOnTheGroundObjective.OnEnterEvent += SpawnSmokeEffect;
        Window.OnShout += RemoveSmokeEffect;
    }

    private void SpawnSmokeEffect()
    {
        if (!smokeEnabled)
        {
            smokeEnabled = true;

            activeSmoke = Instantiate(smokePrefab, playerPosition.transform.position, Quaternion.identity);
            activeSmoke.GetComponent<VisualEffect>().SetFloat("height", BoundsManager.Instance.GetSmokeHeight);
            StartCoroutine(SmokeFade(0.0f, 1.0f, 1.5f, false));

            
        }
        
    }

    private void Update()
    {
        // Only reposition smoke when it is active and a linked gameobject
        if (smokeEnabled && activeSmoke != null)
        {
            activeSmoke.transform.position = new Vector3(playerPosition.transform.position.x, activeSmoke.transform.position.y, playerPosition.transform.position.z);
        }
    }

    private void RemoveSmokeEffect()
    {
        if (smokeEnabled)
        {
            StartCoroutine(SmokeFade(1.0f, 0.0f, 5.0f, true));
            smokeEnabled = false;
        }
    }

    IEnumerator SmokeFade(float start, float end, float time, bool destroyAfter)
    {
        float currentTime = 0f;
        float strength = start;

        while (currentTime < time)
        {
            //Update current lerp
            strength = Mathf.Lerp(start, end, (currentTime / time));
            currentTime += Time.deltaTime;

            // Set smoke VFX strength
            activeSmoke.GetComponent<VisualEffect>().SetFloat("Strength", strength);
            yield return null;
        }

        if (destroyAfter)
        {
            DestroyImmediate(activeSmoke);
        }
        
    }

}
