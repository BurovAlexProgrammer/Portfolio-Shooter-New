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
            RemoveListener<BootAppInitializedEvent>();
            RemoveListener<ShowMainMenuEvent>();
            RemoveListener<RestartGameEvent>();
            RemoveListener<GameContextInitializedEvent>();
            base.RemoveEventHandlers();
        }

        public override void AddEventHandlers()
        {
            base.AddEventHandlers();
            AddListener<BootAppInitializedEvent>(StartupSystemsInitialized);
            AddListener<ShowMainMenuEvent>(ShowMainMenu);
            AddListener<RestartGameEvent>(OnRestartGame);
            AddListener<GameContextInitializedEvent>(OnGameBootInitialized);
        }

        private void OnGameBootInitialized(BaseEvent obj)
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
            if (_gameStateService.CurrentStateIs(GameState.CustomScene))
            {
                return;
            }
            _sceneLoader.LoadSceneAsync(SceneName.Intro).Forget();
        }
    }
}