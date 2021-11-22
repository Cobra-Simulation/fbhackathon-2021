using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spray : MonoBehaviour
{
    public enum Type
    {
        water,
        carbonDioxide,
        foam,
        wetChemical
    }
    public Type sprayType;
    public float power = 0;
    protected float lastPower = 0;
    public bool narrows = false;
    protected virtual void Start()
    {
        UpdatePower();
    }

    protected virtual void Update()
    {
        if (power != lastPower)
        {
            UpdatePower();
        }
    }

    protected virtual void UpdatePower()
    {
        lastPower = power;
    }

    protected virtual void UpdateParticles(ParticleSystem particleSystem, float normalizedPower)
    {
        var main = particleSystem.main;
        main.startSpeed = Mathf.Lerp(0, 500, normalizedPower);
        var em = particleSystem.emission;
        em.rateOverTime = Mathf.Lerp(0, 2500, normalizedPower);
        UpdateShape(particleSystem, normalizedPower);
    }

    protected virtual void UpdateShape(ParticleSystem particleSystem, float normalizedPower)
    {
        var sh = particleSystem.shape;
        if (narrows)
        {
            sh.angle = Mathf.Lerp(10, .5f, normalizedPower);
            sh.radius = Mathf.Lerp(0, .5f, normalizedPower);
        }
        else
        {
            sh.angle = Mathf.Lerp(1, 10f, normalizedPower);
            sh.radius = Mathf.Lerp(0, .5f, normalizedPower);
        }
    }

    protected float NormalizeFloat(float val, float min, float max)
    {
        return (val - min) / (max - min);
    }
}
