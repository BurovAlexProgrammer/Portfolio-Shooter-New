using System;
using UnityEngine;

namespace Game.Health
{
    public abstract class HealthBase : MonoBehaviour
    {
        [SerializeField] private float _maxValue;
        [SerializeField] private float _currentValue;

        public event Action Dead;
        public event Action<HealthBase> TakenDamage;
        public event Action<HealthBase> Changed;
        public event Action<HealthBase> Healed;

        public float MaxValue => _maxValue;
        public float CurrentValue => _currentValue;

        public void Init(float currentHealth, float maxHealth)
        {
            _maxValue = maxHealth;
            _currentValue = Math.Min(currentHealth, maxHealth);
        }

        protected void SetValue(float value)
        {
            _currentValue = value;
        }

        public void Heal(float value)
        {
            if (value < 0f) throw new Exception("Heal cannot be negative");

            _currentValue += value;
            Changed?.Invoke(this);
            Healed?.Invoke(this);
        }

        public void TakeDamage(float value)
        {
            if (value < 0f) throw new Exception("Damage cannot be negative");

            if (_currentValue <= 0f || value == 0f) return;

            _currentValue -= value;

            Changed?.Invoke(this);
            TakenDamage?.Invoke(this);

            if (_currentValue <= 0f)
            {
                _currentValue = 0f;
                Dead?.Invoke();
            }
        }
    }
}