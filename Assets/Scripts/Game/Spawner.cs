using Cysharp.Threading.Tasks;
using sm_application.Extension;
using sm_application.Service;
using UnityEngine;

namespace Game
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private float _startDelay = 3f;
        [SerializeField] private float _maxSpawnTime = 5f;
        [SerializeField] private AnimationCurve _difficultCurve;
        [SerializeField] private GameObject _prefab;
        [Header("Info")]
        [SerializeField] private float _spawnRate;
        [SerializeField] private float _timer;
        [SerializeField] private float _spawnTimer;
        [SerializeField] private float _timerMinutes;
        
        private Player _player;
        private PoolService _poolService;
        
        private bool _paused;

        private void Awake()
        {
            // _player = Services.Get<Player>();
            _poolService = Services.Get<PoolService>();
        }

        private void OnDisable()
        {
            StopSpawn();
        }

        private void OnDestroy()
        {
        }

        public void StartSpawn()
        {
            enabled = true;
            Spawning().Forget();
        }

        public void StopSpawn()
        {
            enabled = false;
        }

        public void PauseSpawn()
        {
            _paused = true;
        }

        public void ContinueSpawn()
        {
            _paused = false;
        }

        private async UniTask Spawning()
        {
            await _startDelay.WaitInSeconds();

            while (this != null && enabled)
            {
                if (_paused) continue;
                
                _timer += Time.deltaTime;
                _spawnTimer -= Time.deltaTime;
                
                if (_spawnTimer <= 0f)
                {
                    Spawn();
                    SetSpawnTimer();
                }
                
                await UniTask.NextFrame();
            }
        }

        private void SetSpawnTimer()
        {
            _timerMinutes = _timer / 60f;
            _spawnRate = _difficultCurve.Evaluate(_timerMinutes);
            _spawnTimer = 1f / _spawnRate;
            _spawnTimer = Mathf.Min(_spawnTimer, _maxSpawnTime);
        }

        private void Spawn()
        {
            var instance = _poolService.Get(_prefab).GameObject;
            instance.transform.position = new Vector3 {x = Random.Range(-10f, 10f), z = Random.Range(-10f, 10f)};
            instance.gameObject.SetActive(true);
        }

    }
}