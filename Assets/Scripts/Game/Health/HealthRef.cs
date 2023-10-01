using Game.Health;
using UnityEngine;

namespace Game
{
    public class HealthRef : MonoBehaviour
    {
        [SerializeField] private HealthBase _health;

        public HealthBase Health => _health;
    }
}