// #nullable enable

using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using sm_application.Service;
using UnityEngine;

namespace Game.GameState
{
    [UsedImplicitly]
    public class GameStateMachine
    {
        public event Action StateChanged;

        private IGameState _activeState;
        private SceneLoaderService _sceneLoader;
        
        public IGameState ActiveState => _activeState;

        public GameStateMachine()
        {
            _sceneLoader = Services.Get<SceneLoaderService>();
            _activeState = new Initial_GameState();
        }
        
        public async UniTask Start()
        {
            // if (_sceneLoader.IsInitialScene(Scenes.Boot))
            // {
            //     await SetStateAsync<GameStates.Bootstrap>();
            //     await SetStateAsync<GameStates.MainMenu>();
            //     return;
            // }
            //
            // if (_sceneLoader.IsInitialScene(Scenes.MainMenu))
            // {
            //     SetStateAsync<GameStates.MainMenu>().Forget();
            //     return;
            // }
            
            SetStateAsync<GameStates.CustomScene>().Forget();
        }

        public async UniTask SetStateAsync<T>() where T : IGameState
        {
            var newStateType = typeof(T);
            
            if (_activeState?.GetType() == newStateType)
            {
                Debug.Log("GameState Enter: " + newStateType.Name + " (Already entered, skipped)");
                return;
            }

            if (_activeState != null)
            {
                Debug.Log("GameState Exit: " + _activeState.GetType().Name);
                await _activeState.ExitState();
            }

            _activeState = Activator.CreateInstance<T>();

            Debug.Log("GameState Enter: " + _activeState.GetType().Name);
            await _activeState.EnterState();
            
            StateChanged?.Invoke();
        }
    }
}