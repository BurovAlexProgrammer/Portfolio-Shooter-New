using JetBrains.Annotations;
using Service;
using sm_application.Scripts.Main.Events;
using sm_application.Scripts.Main.Service;
using sm_application.Scripts.Main.Systems;

namespace Systems
{
    [UsedImplicitly]
    public class HttpSystem : BaseSystem
    {
        public override void Init()
        {
            base.Init();
        }

        private void OnHttpRequest(BaseEvent baseEvent)
        {
            Services.Get<HttpService>().ExecuteRequest(baseEvent as HttpRequestEvent);
        }

        public override void AddEventHandlers()
        {
            base.AddEventHandlers();
            AddListener<HttpRequestEvent>(OnHttpRequest);
        }

        public override void RemoveEventHandlers()
        {
            RemoveListener<HttpRequestEvent>();
            base.RemoveEventHandlers();
        }
    }
}