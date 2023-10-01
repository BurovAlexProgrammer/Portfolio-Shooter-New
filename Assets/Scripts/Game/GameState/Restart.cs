using Cysharp.Threading.Tasks;
using sm_application.Extension;
using UnityEngine.SceneManagement;

namespace Game.GameState
{
    public static partial class GameStates
    {
        public class RestartGame : IGameState
        {
            public async UniTask EnterState()
            {
                var currentScene = SceneManager.GetActiveScene();
                var newScene = SceneManager.CreateScene("Empty");
                newScene.SetActive(true);
            
                await SceneManager.UnloadSceneAsync(currentScene);
            }
        }
    }
}