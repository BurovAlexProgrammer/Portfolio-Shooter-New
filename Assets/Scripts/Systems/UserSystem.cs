using System.Collections.Generic;
using Events;
using Game.DTO;
using Game.DTO.Http;
using Game.Service;
using JetBrains.Annotations;
using Newtonsoft.Json;
using sm_application.Events;
using sm_application.Systems;
using UnityEngine.Networking;

namespace Game.Systems
{
    [UsedImplicitly]
    public class UserSystem : BaseSystem
    {
        private UserService _userService;
        
        public override void Init()
        {
            base.Init();
            _userService = sm_application.Service.Services.Get<UserService>();
        }

        public override void AddEventHandlers()
        {
            base.AddEventHandlers();

            AddListener<CheckUserExistRequiredEvent>(CheckUserExistRequiredEvent);
        }

        private void CheckUserExistRequiredEvent(BaseEvent baseEvent)
        {
            var specEvent = baseEvent as CheckUserExistRequiredEvent;

            var fields = new Dictionary<string, string>()
            {
                { RequestField.DeviceUniqueId, specEvent.UniqueDeviceId }
            };

            new HttpRequestEvent(Endpoint.CheckUserUniqueDeviceId, fields)
                .OnSuccess(OnHttpResponseCheckUniqueDeviceId)
                .Fire();
        }


        private void OnHttpResponseCheckUniqueDeviceId(DownloadHandler downloadHandler)
        {
            var userData = JsonConvert.DeserializeObject<UserData>(downloadHandler.text);
            
            if (userData != default)
            {
                new ExistUserConnectedEvent().Fire();
            }
            else
            {
                new NewUserConnectedEvent().Fire();
            }
        }

        public override void RemoveEventHandlers()
        {
            base.RemoveEventHandlers();
        }
    }
}