using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSpray : Spray
{
    private float damage = 5;
    ParticleSystem waterPs, mistPs, dropsPs;
    ParticleSystemRenderer dropsRend;
    AudioSource audio;
    public void UpdatePressure(float val)
    {
        power = NormalizeFloat(val, 0, 45) * 100;
        UpdatePower();
    }
    protected override void Start()
    {
        audio = GetComponent<AudioSource>();
        waterPs = GetComponent<ParticleSystem>();
        mistPs = transform.GetChild(0).GetComponent<ParticleSystem>();
        dropsPs = transform.GetChild(1).GetComponent<ParticleSystem>();
        dropsRend = transform.GetChild(1).GetComponent<ParticleSystemRenderer>();
        base.Start();
    }

    protected override void UpdatePower()
    {
        base.UpdatePower();
        float normalizedPower = NormalizeFloat(power, 0, 100);
        audio.volume = Mathf.Lerp(0, 1, normalizedPower);
        audio.pitch = Mathf.Lerp(1, 1.25f, normalizedPower);
        UpdateParticles(waterPs, normalizedPower);
        UpdateParticles(mistPs, normalizedPower);
        UpdateParticles(dropsPs, normalizedPower);
        dropsRend.lengthScale = Mathf.Lerp(1, 20, normalizedPower);
    }

    void OnParticleCollision(GameObject other)
    {
        // Water knockback
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float normalizedPower = NormalizeFloat(power, 0, 100);
            List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
            int numCollisionEvents = waterPs.GetCollisionEvents(other, collisionEvents);

            for (int i = 0; i < numCollisionEvents; i++)
            {
                Vector3 force = collisionEvents[i].velocity * Mathf.Lerp(.01f, .5f, normalizedPower);
                rb.AddForce(force);
            }
        }
    }
}
