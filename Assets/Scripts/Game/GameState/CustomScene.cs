using Cysharp.Threading.Tasks;
using sm_application.Service;

namespace Game.GameState
{
    public static partial class GameStates
    {
        public class CustomScene : IGameState
        {
            private GameManagerService _gameManager;

            public async UniTask EnterState()
            {
                _gameManager ??= Services.Get<GameManagerService>();
                await UniTask.Yield();
                _gameManager.PrepareToPlay();
            }
        }
    }
}