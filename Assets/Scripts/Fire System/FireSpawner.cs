using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FireTypes;

public class FireSpawner : MonoBehaviour
{
    public GameObject[] spawnPoints;

    public GameObject firePrefab;

    // Update is called once per frame
    private void Start()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            FireManager.Instance.SpawnFire(new Fire(Instantiate(firePrefab, spawnPoints[i].transform), 0.1f));
        }        
    }
}
