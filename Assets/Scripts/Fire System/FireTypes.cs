using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

namespace FireTypes 
{ 
    public enum FireProperties 
    { 
        Static,
        Spreadable,
        Scripted
    }

    public enum FireSource
    {
        Generic,
        Combustable,
        Metal,
        Oil,
        Liquid,
        Electrical,
        Chemical
    }

    public class Fire
    {
        #region CONSTRUCTORS
        public Fire(GameObject fireInstance, float growRate)
        {
            _instance = fireInstance;
            _growRate = growRate;

            // Grab all visual effect components from the fire prefab
            _visualEffects = fireInstance
                .GetComponents<VisualEffect>()
                .Where(x => x.HasFloat("strength"))
                .ToArray();

            if (_visualEffects.Length < 1 || _visualEffects == null)
            {
                Debug.LogWarning("No visual effect components attached to the fire prefab: " + fireInstance.name);
            }

            _health = 100.0f;
            _maxHealth = _health;
            Source = FireSource.Generic;
            Properties = FireProperties.Static;
            _hasLostHealthThisTick = false;
        }
        public Fire(GameObject fireInstance, float growRate, float health)
        {
            _instance = fireInstance;
            _growRate = growRate;

            // Grab all visual effect components from the fire prefab
            _visualEffects = fireInstance
                .GetComponents<VisualEffect>()
                .Where(x => x.HasFloat("strength"))
                .ToArray();

            if (_visualEffects.Length < 1 || _visualEffects == null)
            {
                Debug.LogWarning("No visual effect components attached to the fire prefab: " + fireInstance.name);
            }

            _health = health;
            _maxHealth = _health;
            Source = FireSource.Generic;
            Properties = FireProperties.Static;
            _hasLostHealthThisTick = false;
        }
        public Fire(GameObject fireInstance, float growRate, float health, float maxHealth)
        {
            _instance = fireInstance;
            _growRate = growRate;

            // Grab all visual effect components from the fire prefab
            _visualEffects = fireInstance
                .GetComponents<VisualEffect>()
                .Where(x => x.HasFloat("strength"))
                .ToArray();

            if (_visualEffects.Length < 1 || _visualEffects == null)
            {
                Debug.LogWarning("No visual effect components attached to the fire prefab: " + fireInstance.name);
            }

            _health = health;
            _maxHealth = maxHealth;
            Source = FireSource.Generic;
            Properties = FireProperties.Static;
            _hasLostHealthThisTick = false;
        }
        public Fire(GameObject fireInstance, float growRate, float health, float maxHealth, FireSource source)
        {
            _instance = fireInstance;
            _growRate = growRate;

            // Grab all visual effect components from the fire prefab
            _visualEffects = fireInstance
                .GetComponents<VisualEffect>()
                .Where(x => x.HasFloat("strength"))
                .ToArray();

            if (_visualEffects.Length < 1 || _visualEffects == null)
            {
                Debug.LogWarning("No visual effect components attached to the fire prefab: " + fireInstance.name);
            }

            _health = health;
            _maxHealth = maxHealth;
            Source = source;
            Properties = FireProperties.Static;
            _hasLostHealthThisTick = false;
        }
        #endregion

        #region DATA
        // Fire settings
        public FireSource Source;
        public FireProperties Properties;

        // Health stats
        private float _maxHealth;
        private float _health;
        private bool _hasLostHealthThisTick;

        // Fire Asset elements
        private VisualEffect[] _visualEffects;
        private float _growRate;
        private GameObject _instance;

        public float Health 
        {
            get { return _health; }
        }
        public float MaxHealth 
        {
            get { return _maxHealth; }
        }
        public bool IsAlive
        {
            get { return _health > 0.0f ? true : false; }
        }
        public GameObject Instance
        {
            get { return _instance; }
        }
        public VisualEffect[] VFX
        {
            get { return _visualEffects; }
        }
        #endregion 

        // Fire tick which is called at a set tick rate by a handler/manager class
        public void Tick()
        {
            if (IsAlive)
            {
                // Grow the fire if it hasn't been extinguished this tick
                if (!_hasLostHealthThisTick) { GrowFire(); }

                // Reset health loss check
                _hasLostHealthThisTick = false;
            }            
        }

        // Checks if a spray type can be used with this fire's source
        bool CanBeUsed(Spray.Type type)
        {
            // Get all active fire rules
            FireRulesData[] data = FireManager.Instance.fireRules.Where(x => x.sprayType == type).ToArray();

            // Run through each fire rule
            for (int i = 0; i < data.Length; i++)
            {
                // Check inside each fire rule for a source the same as this fire's source
                for (int j = 0; j < data[i].fireSources.Length; j++)
                {
                    if (data[i].fireSources[j] == Source)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Extinguish(Spray.Type sprayType, float amount)
        {
            amount = Mathf.Abs(amount);

            if (CanBeUsed(sprayType))
            {
                if (_health - amount < 0.0f)
                {
                    Kill();
                }
                else
                {
                    _health -= amount;
                }
            }
            else
            {
                Debug.Log("Wrong spray used for this fire!");
                _health = _maxHealth;
            }
                        

            // Update the particle effect for each strength percentage
            UpdateParticleStrength();
            _hasLostHealthThisTick = true;
        }
        public void Kill()
        {
            _health = 0.0f;

            // Update the particle effect for each strength percentage
            UpdateParticleStrength();
        }

        private void GrowFire()
        {
            if (_health + _growRate > _maxHealth)
            {
                _health = _maxHealth;
            }
            else 
            {
                _health += _growRate;
            }

            // Update the particle effect for each strength percentage
            UpdateParticleStrength();

        }
        private void UpdateParticleStrength()
        {
            // Update the particle effect for each strength percentage
            for (int i = 0; i < _visualEffects.Length; i++)
            {
                _visualEffects[i].SetFloat("strength", (_health / _maxHealth));
            }
        }

    }




}