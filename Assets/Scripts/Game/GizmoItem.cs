using sm_application.Extension;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace sm_application.Game
{
    public class GizmoItem : MonoBehaviour
    {
        public float ShowTime = 1f;

        private void Start()
        {
            DestroyAfter(ShowTime).Forget();
        }

        private async UniTask DestroyAfter(float time)
        {
            await time.WaitInSeconds();
            
            Destroy(gameObject);
        }
    }
}