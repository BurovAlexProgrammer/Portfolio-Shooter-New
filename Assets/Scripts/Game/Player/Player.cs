using System;
using Game.DTO;
using Game.Health;
using Game.Service;
using Game.Weapon;
using sm_application;
using sm_application.Audio;
using sm_application.Service;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(HealthBase))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public sealed class Player : MonoBehaviour, IPlayer
    {
        [SerializeField] private CameraHolder _cameraHolder;
        [SerializeField] private PlayerConfig _config;
        [SerializeField] private GunBase _gun;
        [SerializeField] private bool _canMove;
        [SerializeField] private bool _canRotate;
        [SerializeField] private bool _canShoot;
        [SerializeField] private bool _useGravity;
        [SerializeField] private SimpleAudioEvent _startPhrase;

        private StatisticService _statisticService;
        private ControlService _controlService;
        private SettingsService _settingsService;
        private CharacterController _characterController;
        private AudioSource _audioSource;
        private HealthBase _health;
        private Controls.PlayerActions _playerControl;
        private Vector2 _moveInputValue;
        private Vector2 _moveLerpValue;
        private Vector2 _rotateInputValue;
        private Vector2 _rotateLerpValue;
        private float _rotationY;
        private bool _shootInputValue;
        private Vector3 _playerMove;

        public CameraHolder CameraHolder => _cameraHolder;
        public HealthBase Health => _health;

        private void Awake()
        {
            _statisticService = Services.Get<StatisticService>();
            _controlService = Services.Get<ControlService>();
            _settingsService = Services.Get<SettingsService>();
            _health = GetComponent<HealthBase>();
            _characterController = GetComponent<CharacterController>();
            _audioSource = GetComponent<AudioSource>();
            _playerControl = _controlService.Controls.Player;
            _controlService.SetPlayMode();
        }

        private void Start()
        {
            if (_startPhrase != null)
            {
                _startPhrase.Play(_audioSource);
            }
            else
            {
                Debug.LogWarning("Player does not have Start Phrase. (Click to select)", this);
            }
        }

        private void Update()
        {
            _rotateInputValue = _playerControl.Rotate.ReadValue<Vector2>();
            _rotateLerpValue = Vector2.Lerp(_rotateLerpValue, _rotateInputValue, _config.RotateLerpTime);

            if (_canRotate)
            {
                Rotate(_rotateLerpValue);
            }

            if (_canShoot && _playerControl.Shoot.IsPressed())
            {
                TryShoot();
            }
        }

        private void FixedUpdate()
        {
            _moveInputValue = _playerControl.Move.ReadValue<Vector2>().normalized;
            _moveLerpValue = Vector2.Lerp(_moveLerpValue, _moveInputValue, _config.MoveLerpTime);

            if (_canMove)
            {
                Move(_moveLerpValue);
            }
        }

        public void Disable()
        {
            _canMove = false;
            _canRotate = false;
            _canShoot = false;
        }

        public void Enable()
        {
            _canMove = true;
            _canRotate = true;
            _canShoot = true;
        }

        public void Move(Vector2 inputValue)
        {
            if (!_canMove) return;

            var moveVector = inputValue * Time.fixedDeltaTime * _config.MoveSpeed;
            var gravityVelocity = _useGravity ? Physics.gravity * Time.fixedDeltaTime : Vector3.zero;
            _characterController.Move(transform.right * moveVector.x + transform.forward * moveVector.y +
                                      gravityVelocity);

            if (_characterController.velocity != Vector3.zero)
            {
                _statisticService.AddValue(StatisticData.Movement,
                    _characterController.velocity.magnitude * Time.fixedDeltaTime);
            }
        }

        public void Rotate(Vector2 inputValue)
        {
            if (!_canMove) return;
            if (inputValue == Vector2.zero) return;

            var delta = Time.deltaTime * _config.RotateSpeed * _settingsService.GameSettings.Sensitivity;
            _rotationY -= inputValue.y * delta;
            _rotationY = Math.Clamp(_rotationY, -_config.MaxVerticalAngle, _config.MaxVerticalAngle);
            _cameraHolder.transform.localRotation = Quaternion.Euler(_rotationY, 0f, 0f);
            transform.Rotate(inputValue.x * Vector3.up * delta);
        }

        private void TryShoot()
        {
            if (_gun.TryShoot())
            {
                _statisticService.AddValue(StatisticData.FireCount, 1);
            }
        }
    }
}