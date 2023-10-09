using Game.DTO;
using Game.Health;
using Game.Service;
using Game.Weapon;
using sm_application;
using sm_application.Service;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(HealthBase))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public sealed class Player : MonoBehaviour
    {
        [SerializeField] private PlayerMoveController _moveController;
        [SerializeField] private PlayerAnimatorController _animatorController;
        
        [SerializeField] private CameraHolder _cameraHolder;
        [SerializeField] private PlayerConfig _config;
        [SerializeField] private GunBase _gun;
        
        private StatisticService _statisticService;
        private ControlService _controlService;
        private SettingsService _settingsService;
        private CharacterController _characterController;
        private AudioSource _audioSource;
        private HealthBase _health;


        public CameraHolder CameraHolder => _cameraHolder;
        public HealthBase Health => _health;
        public PlayerConfig Config => _config;

        private void Awake()
        {
            _statisticService = Services.Get<StatisticService>();
            _controlService = Services.Get<ControlService>();
            _settingsService = Services.Get<SettingsService>();
            _health = GetComponent<HealthBase>();
            _characterController = GetComponent<CharacterController>();
            _audioSource = GetComponent<AudioSource>();
            _moveController.Setup(this, _characterController);
            _controlService.SetPlayMode();
            _moveController.SpeedChanged += OnSpeedChanged;
        }

        private void OnSpeedChanged(float speed)
        {
            _animatorController.SetSpeed(speed);
        }

        private void OnDestroy()
        {
            _moveController.SpeedChanged -= OnSpeedChanged;

        }

        public void OnDisable()
        {
            _moveController.enabled = false;
        }

        public void OnEnable()
        {
            _moveController.enabled = true;
        }

        private void OnInputRotate(Vector2 vector2)
        {
            _moveController.Rotate(vector2);
        }

        private void OnInputMove(Vector2 vector2)
        {
            _moveController.Move(vector2);
        }


        private void TryAttack()
        {
            if (_gun.TryShoot())
            {
                _statisticService.AddValue(StatisticData.FireCount, 1);
            }
        }
    }
}