using Game.Service;
using JetBrains.Annotations;
using sm_application.Events;
using sm_application.Service;
using sm_application.Systems;

namespace Game.Systems
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
            sm_application.Service.Services.Get<HttpService>().ExecuteRequest(baseEvent as HttpRequestEvent);
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