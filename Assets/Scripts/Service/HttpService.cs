using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DTO.Http;
using Duck.Http;
using Duck.Http.Service.Unity;
using Events;
using JetBrains.Annotations;
using Newtonsoft.Json;
using sm_application.Scripts.Main.Events;
using sm_application.Scripts.Main.Service;
using sm_application.Scripts.Main.Wrappers;
using UnityEngine;
using UnityEngine.Networking;

namespace Service
{
    [UsedImplicitly]
    public class HttpService : IService, IConstruct
    {
        private string _defaultServer = "http://localhost:8000";
        
        public void Construct()
        {
            Http.Init(new UnityHttpService());
        }

        public void GetTimeNow()
        {
            Http.Get(string.Concat(_defaultServer, "/api/user/now")).Send();
        }

        public void CreateUser()
        {
            var dic = new Dictionary<string, string>();
            dic.Add("authKey", "testKey2");
            dic.Add("nickname", "nickname2");
            var req = UnityWebRequest.Post(string.Concat(_defaultServer, Endpoint.UserCreate), dic);
            req.Send();
        }

        public async void ExecuteRequest(HttpRequestEvent httpRequestEvent)
        {
            UnityWebRequest request = null;
            
            var uri = string.Concat(_defaultServer, httpRequestEvent.Endpoint);
            
            switch (httpRequestEvent.HttpMethod)
            {
                case HttpRequestMethod.Get:
                    request = UnityWebRequest.Get(uri);
                    break;
                case HttpRequestMethod.Post:
                    request = UnityWebRequest.Post(uri, httpRequestEvent.Fields);
                    break;
                case HttpRequestMethod.Remove:
                    request = UnityWebRequest.Delete(uri);
                    break;
                case HttpRequestMethod.Put:
                    request = UnityWebRequest.Put(uri, JsonConvert.SerializeObject(httpRequestEvent.Fields));
                    break;
            }

            var operation = request.SendWebRequest();
            operation.completed += OnRequestReturned;
        }

        private void OnRequestReturned(AsyncOperation operation)
        {
            var webOperation = operation as UnityWebRequestAsyncOperation;
            switch (webOperation.webRequest.result)
            {
                case UnityWebRequest.Result.InProgress:
                    Log.ErrorUnknown();
                    break;
                case UnityWebRequest.Result.Success:
                    //TODO use DUCK
                    break;
                case UnityWebRequest.Result.ConnectionError:
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Debug.LogWarning("Done");
        }
    }
}