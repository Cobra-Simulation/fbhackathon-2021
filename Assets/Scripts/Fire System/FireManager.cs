using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FireTypes;

public class FireManager : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("The speed in seconds of each fire tick.")]
    public float fireTickRateTime = 1.0f;
    private WaitForSeconds fireTickRate = new WaitForSeconds(1.0f);

    [Header("Assets")]
    public GameObject[] firePrefabs;
    private List<Fire> activeFires = new List<Fire>();

    [Header("Fire Rules")]
    public FireRulesData[] fireRules;


    #region GETTERS
    public int ActiveFires
    {
        get { return activeFires.Count; }
    }

    public int ActiveScriptedFires
    {
        get 
        {
            return activeFires.FindAll(X => X.Properties == FireProperties.Scripted).Count;
        }
    }

    public bool IsFireActive
    {
        get 
        {
            return activeFires.Count > 0 ? true : false;
        }
    }
    #endregion

    #region SINGLETON MANAGEMENT
    private static FireManager _instance;
    public static FireManager Instance { get { return _instance; } }

    private void Awake()
    {
        fireTickRate = new WaitForSeconds(fireTickRateTime);
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        StopAllCoroutines();
        StartCoroutine(RunFireTick());
    }
    #endregion


    #region SPAWN RANDOM FIRES
    public void SpawnRandomFire(Vector3 position)
    {
        GameObject asset = firePrefabs[Random.Range(0, firePrefabs.Length - 1)];
        SpawnFire(new Fire(Instantiate(asset, position, Quaternion.identity), 0.1f));
    }
    public void SpawnRandomFires(GameObject[] spawnPoints)
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject asset = firePrefabs[Random.Range(0, firePrefabs.Length - 1)];
            SpawnFire(new Fire(Instantiate(asset, spawnPoints[i].transform.position, Quaternion.identity), 0.1f));
        }        
    }
    public void SpawnRandomFires(Vector3[] spawnPoints)
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject asset = firePrefabs[Random.Range(0, firePrefabs.Length - 1)];
            SpawnFire(new Fire(Instantiate(asset, spawnPoints[i], Quaternion.identity), 0.1f));
        }
    }
    #endregion

    // Fire update / tick method
    private IEnumerator RunFireTick()
    {
        yield return fireTickRate;
        for (int i = 0; i < activeFires.Count; i++)
        {
            if (activeFires[i].IsAlive)
            {
                activeFires[i].Tick();
            }
            else
            {
                Debug.Log("Killed fire: " + activeFires[i].Instance.name);
                FireManager.Instance.RemoveFireInstance(activeFires[i].Instance);
            }
        }
        StartCoroutine(RunFireTick());
    }

    public void SpawnFire(Fire fire)
    {
        activeFires.Add(fire);
    }

    public void Reset()
    {
        for (int i = 0; i < activeFires.Count; i++)
        {
            DestroyImmediate(activeFires[i].Instance);           
        }
        activeFires.Clear();
    }
    public void RemoveFireInstance(GameObject fire)
    {
        for (int i = 0; i < activeFires.Count; i++)
        {
            if (activeFires[i].Instance == fire)
            {
                activeFires.RemoveAt(i);
                DestroyImmediate(fire);
            }
        }
    }
    public void DestroyFire(int index)
    {
        activeFires.RemoveAt(index);
    }

    public void Extinguish(GameObject fire, Spray.Type sprayType ,float amount)
    {
        for (int i = 0; i < activeFires.Count; i++)
        {
            if (activeFires[i].Instance == fire)
            {
                activeFires[i].Extinguish(sprayType, amount);
            }
        }
    }


}

