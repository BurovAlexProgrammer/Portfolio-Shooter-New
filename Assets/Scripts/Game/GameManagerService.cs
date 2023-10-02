using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.GameState;
using Game.Service;
using sm_application.Extension;
using sm_application.Service;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class GameManagerService : IService
    {
        private GameStateMachine _gameStateMachine;
        private bool _isGamePause;
        private int _scores;

        private ControlService _controlService;
        private SceneLoaderService _sceneLoader;
        private StatisticService _statisticService;
        private AudioService _audioService;

        public event Action<bool> SwitchPause;
        public event Action GameOver;

        private bool _transaction;
        private bool _isGameOver;

        public IGameState ActiveGameState => _gameStateMachine.ActiveState;
        public bool IsGamePause => _isGamePause;
        public bool IsGameOver => _isGameOver;
        public int Scores => _scores;

        public void Construct()
        {
            _controlService = Services.Get<ControlService>();
            _sceneLoader = Services.Get<SceneLoaderService>();
            _statisticService = Services.Get<StatisticService>();
            _audioService = Services.Get<AudioService>();
            _gameStateMachine = new GameStateMachine();
            _controlService.Controls.Player.Pause.BindAction(BindActions.Started, PauseGame);
            _gameStateMachine.Start().Forget();
        }

        public async UniTask SetGameState<T>() where T : IGameState
        {
            await _gameStateMachine.SetStateAsync<T>();
        }

        public void RestartGame()
        {
            _isGameOver = false;
            RestoreTimeSpeed();
            // _statisticService.EndGameDataSaving(this);
            _gameStateMachine.SetStateAsync<GameStates.RestartGame>().Forget();
            _gameStateMachine.SetStateAsync<GameStates.PlayNewGame>().Forget();
        }

        public void QuitGame()
        {
            _gameStateMachine.SetStateAsync<GameStates.QuitGame>().Forget();
        }

        public void GoToMainMenu()
        {
            // _statisticService.EndGameDataSaving(this);
            _gameStateMachine.SetStateAsync<GameStates.MainMenu>().Forget();
        }

        public void PrepareToPlay()
        {
            _audioService.PlayMusic(AudioService.MusicPlayerState.Battle);
            // _controlService.LockCursor();
            _controlService.Controls.Player.Enable();
            _controlService.Controls.Menu.Disable();
            _statisticService.ResetSession();
        }

        public async void PauseGame(InputAction.CallbackContext ctx)
        {
            if (_transaction) return;

            if (ActiveStateEquals<GameStates.PlayNewGame>() == false &&
                ActiveStateEquals<GameStates.CustomScene>() == false) return;

            Debug.Log("Game paused to menu.");

            var fixedDeltaTime = Time.fixedDeltaTime;
            _transaction = true;
            _isGamePause = true;
            _controlService.Controls.Player.Disable();
            // _controlService.UnlockCursor();
            SwitchPause?.Invoke(_isGamePause);

            await FluentSetTimeScale(0f);

            _controlService.Controls.Menu.Enable();
            Time.fixedDeltaTime = fixedDeltaTime;
            _transaction = false;
        }

        public async void ReturnGame()
        {
            if (_isGameOver) return;
            if (_transaction) return;

            if (ActiveStateEquals<GameStates.PlayNewGame>() == false &&
                ActiveStateEquals<GameStates.CustomScene>() == false) return;

            Debug.Log("Game returned from pause.");
            _transaction = true;
            _isGamePause = false;
            SwitchPause?.Invoke(_isGamePause);
            _controlService.Controls.Player.Enable();
            // _controlService.LockCursor();

            await FluentSetTimeScale(1f);

            _controlService.Controls.Menu.Disable();
            _transaction = false;
        }

        public async void RunGameOver()
        {
            Debug.Log("Game Over");
            // _statisticService.EndGameDataSaving(this);
            _controlService.Controls.Player.Disable();

            await FluentSetTimeScale(1f);

            // _controlService.UnlockCursor();
            _controlService.Controls.Menu.Enable();

            _isGameOver = true;
            GameOver?.Invoke();
        }

        public bool ActiveStateEquals<T>() where T : IGameState
        {
            return ActiveGameState.EqualsState(typeof(T));
        }

        public void RestoreTimeSpeed()
        {
            SetTimeScale(1f);
        }

        private void AddScores(int value)
        {
            if (value < 0)
            {
                throw new Exception("Adding scores cannot be below zero.");
            }

            _scores += value;
            _statisticService.SetScores(_scores);
        }

        private void AddScoresOnCharacterDead(Character character)
        {
            // AddScores(characterController.Data.Score);
        }

        private void ReturnGame(InputAction.CallbackContext ctx)
        {
            ReturnGame();
        }

        private async UniTask FluentSetTimeScale(float scale)
        {
            var timeScale = Time.timeScale;
            await DOVirtual.Float(timeScale, scale, 1f, SetTimeScale)
                .SetUpdate(true)
                .AsyncWaitForCompletion();
        }

        private void SetTimeScale(float value)
        {
            Time.timeScale = value;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }
}