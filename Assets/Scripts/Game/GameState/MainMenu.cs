using Cysharp.Threading.Tasks;
using Game.Constants;
using sm_application.Events;
using sm_application.Service;

namespace Game.GameState
{
    public static partial class GameStates
    {
        public class MainMenu : IGameState
        {
            private AudioService _audioService;
            private SceneLoaderService _sceneLoaderService;

            public async UniTask EnterState()
            {
                _audioService = Services.Get<AudioService>();
                _sceneLoaderService = Services.Get<SceneLoaderService>();
                _audioService.PlayMusic(AudioService.MusicPlayerState.MainMenu);
                new RequireLoadSceneEvent() {NextSceneName = SceneName.MainMenu}.Fire();
            }
        }
    }
}