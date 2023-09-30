using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Events;
using Game.DTO.Constants;
using JetBrains.Annotations;
using sm_application.Events;
using sm_application.Service;
using sm_application.Systems;
using sm_application.Wrappers;

namespace Systems
{
    [UsedImplicitly]
    public class SceneLoaderSystem : BaseSystem
    {
        private SceneLoaderService _sceneLoader;
        private GameStateService _gameStateService;
        private HardwareService _hardwareService;
        private ScreenService _screenService;

        public override void Init()
        {
            base.Init();
            _sceneLoader = Services.Get<SceneLoaderService>();
            _gameStateService = Services.Get<GameStateService>();
            _hardwareService = Services.Get<HardwareService>();
            _screenService = Services.Get<ScreenService>();
        }

        public override void RemoveEventHandlers()
        {
            RemoveListener<BootAppInitializedEvent>();
            RemoveListener<ShowMainMenuEvent>();
            RemoveListener<ReloadSceneEvent>();
            RemoveListener<GameContextInitializedEvent>();
            RemoveListener<RequireLoadSceneEvent>();
            base.RemoveEventHandlers();
        }

        public override void AddEventHandlers()
        {
            base.AddEventHandlers();
            AddListener<BootAppInitializedEvent>(StartupSystemsInitialized);
            AddListener<ShowMainMenuEvent>(ShowMainMenu);
            AddListener<ReloadSceneEvent>(ReloadScene);
            AddListener<GameContextInitializedEvent>(GameBootInitialized);
            AddListener<RequireLoadSceneEvent>(RequireLoadScene);
        }

        private void LoadScene(string sceneName, bool isForce)
        {
            var newEvent = new RequireLoadSceneEvent()
            {
                NextSceneName = sceneName,
                ForceAppearance = isForce
            };
            RequireLoadScene(newEvent);
        }

        private async void RequireLoadScene(BaseEvent baseEvent)
        {
            var currEvent = baseEvent as RequireLoadSceneEvent;

            if (currEvent.ForceAppearance)
            {
                _screenService.SetTopFrameVisible(true);
            }
            else
            {
                await _screenService.SoftTopFrameVisibleAsync(true, currEvent);
            }

            _sceneLoader.LoadScene(currEvent.NextSceneName);

            if (currEvent.ForceAppearance)
            {
                _screenService.SetTopFrameVisible(false);
            }
            else
            {
                await _screenService.SoftTopFrameVisibleAsync(false, currEvent);
            }

            new SceneLoadedEvent().Fire();
        }

        private void GameBootInitialized(BaseEvent obj)
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

        private void ReloadScene(BaseEvent obj)
        {
            _sceneLoader.ReloadActiveScene();
        }

        private void ShowMainMenu(BaseEvent obj)
        {
            RequireLoadScene(new RequireLoadSceneEvent() { NextSceneName = SceneName.MainMenu });
        }

        private void StartupSystemsInitialized(BaseEvent evnt)
        {
            Log.Info("Initialized");

            if (_gameStateService.CurrentStateIs(GameState.CustomScene)) return;

            new RequireLoadSceneEvent() { NextSceneName = SceneName.Intro }.Fire();
        }
    }
}