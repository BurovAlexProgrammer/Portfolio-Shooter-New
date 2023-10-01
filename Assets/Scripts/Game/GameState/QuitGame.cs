using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.GameState
{
    public static partial class GameStates
    {
        public class QuitGame : IGameState
        {
            public async UniTask EnterState()
            {
                await UniTask.Yield();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }
}