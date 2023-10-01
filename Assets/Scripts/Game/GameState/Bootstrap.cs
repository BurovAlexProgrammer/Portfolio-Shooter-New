using Cysharp.Threading.Tasks;
using sm_application.Extension;
using sm_application.Service;

namespace Game.GameState
{
    public static partial class GameStates
    {
        public class Bootstrap : IGameState
        {
            private SceneLoaderService _sceneLoaderService;

            public async UniTask EnterState()
            {
                _sceneLoaderService = Services.Get<SceneLoaderService>();
                await 3f.WaitInSeconds();
            }
        }
    }
}