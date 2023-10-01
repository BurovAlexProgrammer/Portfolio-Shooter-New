using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Constants;
using Game.Service;
using sm_application.Events;
using sm_application.Service;
using UnityEngine;

namespace Game.GameState
{
    public static partial class GameStates
    {
        public class PlayNewGame : IGameState
        {
            private GameManagerService _gameManager;
            private AudioService _audioService;
            private ControlService _controlService;
            private SceneLoaderService _sceneLoaderService;
            private StatisticService _statisticService;

            public async UniTask EnterState()
            {
                _gameManager ??= Services.Get<GameManagerService>();
                _audioService ??= Services.Get<AudioService>();
                _controlService ??= Services.Get<ControlService>();
                _sceneLoaderService ??= Services.Get<SceneLoaderService>();
                _statisticService ??= Services.Get<StatisticService>();
                await UniTask.Yield();
                _gameManager.PrepareToPlay();
                new RequireLoadSceneEvent() { NextSceneName = SceneName.Game}.Fire();
            }

            public async UniTask ExitState()
            {
                _audioService.StopMusic();

                if (Time.timeScale == 0f)
                {
                    await DOVirtual.Float(0, 1f, 0.5f, x => Time.timeScale = x).AwaitForComplete();
                }

                // _controlService.UnlockCursor();
                _statisticService.SaveToFile();
            }
        }
    }
}