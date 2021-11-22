using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtinguisherSpray : Spray
{
    public ParticleSystem[] particleSystems;
    public float maxFill = 100, currentFill = 0, maxRate = 250, maxSpeed = 100;
    float lossVal = 0;
    public bool empty = false;
    protected override void Start()
    {
        currentFill = maxFill;
        base.Start();
    }

    protected override void Update()
    {
        if (!empty && enabled)
        {
            base.Update();
            currentFill -= lossVal * Time.deltaTime;
            if (currentFill <= 0)
            {
                foreach (ParticleSystem ps in particleSystems)
                {
                    UpdateParticles(ps, 0);
                }
                empty = true;
            }
        }
    }
    protected override void UpdatePower()
    {
        base.UpdatePower();
        float normalizedPower = NormalizeFloat(power, 0, 100);
        lossVal = Mathf.Lerp(0, 10, normalizedPower);
        foreach (ParticleSystem ps in particleSystems)
        {
            UpdateParticles(ps, normalizedPower);
        }
    }

    protected override void UpdateParticles(ParticleSystem particleSystem, float normalizedPower)
    {
        base.UpdateParticles(particleSystem, normalizedPower);
        var main = particleSystem.main;
        main.startSpeed = Mathf.Lerp(0, maxSpeed, normalizedPower);
        var em = particleSystem.emission;
        em.rateOverTime = Mathf.Lerp(0, maxRate, normalizedPower);
        UpdateShape(particleSystem, normalizedPower);
    }

}
