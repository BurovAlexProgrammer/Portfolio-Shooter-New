using System;
using Game.DTO;
using Game.Service;
using MyBox;
using sm_application.Service;
using UnityEngine;

namespace Game
{
    public class PlayerMoveController: MonoBehaviour
    {
        [SerializeField] private bool _canMove;
        [SerializeField] private bool _canRotate;
        [SerializeField] private bool _canShoot;
        [SerializeField] private bool _useGravity;
        
        [SerializeField][ReadOnly] private float _speed;
        [SerializeField][ReadOnly] private bool _isRun;
        
        private Controls.PlayerActions _playerControl;
        private Player _player;
        private CharacterController _characterController;
        private StatisticService _statisticService;
        private SettingsService _settingsService;
        private Vector2 _moveLerpValue;
        private Vector2 _rotateInputValue;
        private Vector2 _rotateLerpValue;
        private Vector2 _moveInputValue;
        private float _rotationY;

        public event Action<float> SpeedChanged;

        public bool IsRun => _isRun;


        public void Setup(Player player, CharacterController characterController)
        {
            _playerControl = Services.Get<ControlService>().Controls.Player;
            _settingsService = Services.Get<SettingsService>();
            _statisticService = Services.Get<StatisticService>();
            _player = player;
            _characterController = characterController;
        }

        private void Update()
        {
            _rotateInputValue = _playerControl.Rotate.ReadValue<Vector2>();
            _rotateLerpValue = Vector2.Lerp(_rotateLerpValue, _rotateInputValue, _player.Config.RotateLerpTime);
            _isRun = _playerControl.Run.IsInProgress();

            Rotate(_rotateLerpValue);

            if (_playerControl.Shoot.IsPressed())
            {
                //TryAttack(); //TODO move to AttackController
            }
        }

        private void TryAttack()
        {
            throw new NotImplementedException();
        }

        private void FixedUpdate()
        {
            _moveInputValue = _playerControl.Move.ReadValue<Vector2>().normalized;
            _moveLerpValue = Vector2.Lerp(_moveLerpValue, _moveInputValue, _player.Config.MoveLerpTime);
            Move(_moveLerpValue);
        }

        public void Move(Vector2 inputValue)
        {
            if (!_canMove)
            {
                SetSpeed(0f);
                return;
            }

            var moveVector = inputValue * Time.fixedDeltaTime * (IsRun ? _player.Config.RunSpeed : _player.Config.MoveSpeed);
            SetSpeed(moveVector.magnitude * 10f);
            var gravityVelocity = _useGravity ? Physics.gravity * Time.fixedDeltaTime : Vector3.zero;
            _characterController.Move(transform.right * moveVector.x + transform.forward * moveVector.y + gravityVelocity);

            if (_characterController.velocity != Vector3.zero)
            {
                _statisticService.AddValue(StatisticData.Movement,
                    _characterController.velocity.magnitude * Time.fixedDeltaTime);
            }
        }

        public void Rotate(Vector2 inputValue)
        {
            if (!_canRotate) return;
            if (inputValue == Vector2.zero) return;

            var delta = Time.deltaTime * _player.Config.RotateSpeed * _settingsService.GameSettings.Sensitivity;
            _rotationY -= inputValue.y * delta;
            _rotationY = Math.Clamp(_rotationY, - _player.Config.MaxVerticalAngle, _player.Config.MaxVerticalAngle);
            _player.CameraHolder.transform.localRotation = Quaternion.Euler(_rotationY, 0f, 0f);
            transform.Rotate(inputValue.x * Vector3.up * delta);
        }

        private void SetSpeed(float speed)
        {
            if (!speed.Approximately(_speed))
            {
                _speed = speed;
                SpeedChanged(speed);
            }
        }
    }
}