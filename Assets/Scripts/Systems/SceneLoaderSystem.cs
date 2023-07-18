using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DTO.Http;
using Events;
using JetBrains.Annotations;
using Game.DTO.Enums;
using sm_application.Scripts.Main.Events;
using sm_application.Scripts.Main.Service;
using sm_application.Scripts.Main.Systems;
using sm_application.Scripts.Main.Wrappers;
using UnityEngine.Networking;

namespace Systems
{
    [UsedImplicitly]
    public class SceneLoaderSystem : BaseSystem
    {
        private SceneLoaderService _sceneLoader;
        private GameStateService _gameStateService;
        private HardwareService _hardwareService;

        public override void Init()
        {
            base.Init();
            _sceneLoader = Services.Get<SceneLoaderService>();
            _gameStateService = Services.Get<GameStateService>();
            _hardwareService = Services.Get<HardwareService>();
            
            if (_sceneLoader.IsCustomScene())
            {
                _gameStateService.SetState(GameState.CustomScene);
            }
        }
        
        public override void RemoveEventHandlers()
        {
            RemoveListener<StartupSystemsInitializedEvent>();
            RemoveListener<ShowMainMenuEvent>();
            RemoveListener<RestartGameEvent>();
            RemoveListener<StartupGameInitializedEvent>();
            base.RemoveEventHandlers();
        }

        public override void AddEventHandlers()
        {
            base.AddEventHandlers();
            AddListener<StartupSystemsInitializedEvent>(StartupSystemsInitialized);
            AddListener<ShowMainMenuEvent>(ShowMainMenu);
            AddListener<RestartGameEvent>(OnRestartGame);
            AddListener<StartupGameInitializedEvent>(OnStartupGameInitialized);
        }

        private void OnStartupGameInitialized(BaseEvent obj)
        {
            //Services.Get<HttpService>().GetTimeNow();
            //Services.Get<HttpService>().CreateUser();
            var dic = new Dictionary<string, string>();
            dic.Add("authKey", "testKey2");
            dic.Add("nickname", "nickname2");

            // new HttpRequestEvent(Endpoint.UserCreate, dic);
            // new HttpRequestEvent(Endpoint.ServerTime)
            //     .OnSuccess(OnServerTimeSuccess)
            //     .OnResponse(OnServerTimeResponse)
            //     //.OnError(OnTimeRequestError)
            //     .OnTimeOut(Action)
            //     .Fire();
            
            new CheckUserExistRequiredEvent()
            {
                UniqueDeviceId = _hardwareService.UniqueDeviceId
            }.Fire();
        }

        private void OnRestartGame(BaseEvent obj)
        {
            _sceneLoader.ReloadActiveScene();
        }

        private void ShowMainMenu(BaseEvent obj)
        {
            _sceneLoader.LoadSceneAsync(SceneName.MainMenu).Forget();
        }

        private void StartupSystemsInitialized(BaseEvent evnt)
        {
            Log.Info("Initialized");
            // if (_gameStateService.CurrentStateIs(GameState.CustomScene))
            // {
            //     return;
            // }
            // _sceneLoader.LoadSceneAsync(SceneName.Intro).Forget();
        }
    }
}