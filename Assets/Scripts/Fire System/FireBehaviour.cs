using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBehaviour : MonoBehaviour
{

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

        FireManager.Instance.Extinguish(gameObject, sprayType, 0.1f * power * Time.deltaTime);
    }
    
}
