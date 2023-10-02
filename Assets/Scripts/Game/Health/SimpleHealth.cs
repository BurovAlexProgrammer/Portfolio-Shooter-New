using Game.Weapon;
using sm_application.Service;
using UnityEngine;

namespace Game.Health
{
    [DisallowMultipleComponent]
    public class SimpleHealth : HealthBase
    {
        [SerializeField] private Destruction _destructionPrefab;
        
        private PoolService _poolService;
        
        private void Awake()
        {
            _poolService = Services.Get<PoolService>();
        }

        private void OnEnable()
        {
            if (CurrentValue == 0f)
            {
                SetValue(MaxValue);
            }

            Dead += Destruct;
        }

        private void OnDisable()
        {
            Dead -= Destruct;
        }

        private void Destruct()
        {
            if (_destructionPrefab != null)
            {
                var enemyParts = _poolService.GetAndActivate(_destructionPrefab);
                enemyParts.GameObject.transform.position = transform.position;
                enemyParts.GameObject.transform.rotation = transform.rotation;
            }

            Destroy(gameObject);
        }
    }
}