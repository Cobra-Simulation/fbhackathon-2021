using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeFire : MonoBehaviour
{
    public float health = 0, maxHealth = 100;

    private void Start()
    {
        health = maxHealth;
    }

    // Fire Collision START --------------------------------------------------------------------
    Spray lastTarget = null;
    void OnParticleCollision(GameObject other)
    {
        Spray target;
        if (lastTarget == null || other != lastTarget.gameObject)
        {            
            target = other.GetComponent<Spray>();
            lastTarget = target;
        }
        else
        {
            target = lastTarget;
        }
        Spray.Type sprayType = target.sprayType;
        float power = target.power;
        health -= 0.1f;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    // Fire Collision END --------------------------------------------------------------------
}
