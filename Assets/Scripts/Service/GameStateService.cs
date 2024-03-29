using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Constants;
using sm_application.Events;
using sm_application.Extension;
using sm_application.Service;
using sm_application.Wrappers;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Service
{
    public class GameStateService : IService
    {
        public bool IsGamePause;
        public bool IsGameOver;
        private bool _isMenuMode;
        
        private Constants.GameState _currentState;
        
        private SceneLoaderService _sceneLoader;

        public Constants.GameState CurrentState => _currentState;
        public bool IsMenuMode => _isMenuMode;
        

        public void SetPause(bool value)
        {
            IsGamePause = value;
        }
        

        public void SetState(Constants.GameState newState)
        {
            if (_currentState == newState)
            {
                Log.Info($"GameState: {newState.ToString()} (Already entered, skipped)");
                return;
            }
            
            _currentState = newState;
            var color = Common.ThemeColorHex("#39A5E6", "#004F99");
            Log.Info($"GameState: <color={color}> {newState.ToString()}</color>. {DateTime.Now.ToString("hh:mm:ss")}");
        }
        
        public void RestartGame()
        {
            new ReloadSceneEvent().Fire();
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

        public void PrepareToPlay()
        {
            // Old_Services.AudioService.PlayMusic(AudioService.MusicPlayerState.Battle).Forget();
            // Old_Services.ControlService.LockCursor();
            // Old_Services.ControlService.Controls.Player.Enable();
            // Old_Services.ControlService.Controls.Menu.Disable();
            // Old_Services.StatisticService.ResetSessionRecords();
        }

        public void GameOver()
        {
            IsGameOver = true;
        }
        
        public bool CurrentStateIs(params Constants.GameState[] states)
        {
            return states.Any(x => x == CurrentState);
        }
        
        public bool CurrentStateIsNot(params Constants.GameState[] states)
        {
            return states.All(x => x != CurrentState);
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

            // _scores += value;
            // _statisticService.SetScores(_scores);
        }
        
        public async UniTask FluentSetTimeScale(float scale, float duration)
        {
            var fixedDeltaTime = Time.fixedDeltaTime;
            var timeScale = Time.timeScale;
            await DOVirtual.Float(timeScale, scale, duration, SetTimeScale)
                .SetUpdate(true)
                .AsyncWaitForCompletion();
            Time.fixedDeltaTime = fixedDeltaTime;
        }

        public void SetTimeScale(float value)
        {
            Time.timeScale = value;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }

        public void Construct()
        {
        }
    }
}