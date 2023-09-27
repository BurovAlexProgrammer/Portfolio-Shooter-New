using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DTO.Http;
using Duck.Http;
using Duck.Http.Service.Unity;
using Events;
using JetBrains.Annotations;
using Newtonsoft.Json;
using sm_application.Events;
using sm_application.HttpData;
using sm_application.Wrappers;
using UnityEngine;
using UnityEngine.Networking;

namespace sm_application.Service
{
    [UsedImplicitly]
    public class HttpService : IService, IConstruct
    {
        public event Action<bool> ServerStatusChanged;

        private bool _isOnline;
        private List<HttpRequestEvent> _executingRequests = new List<HttpRequestEvent>(5);

        private const string DefaultServer = "http://localhost:8000";
        private const float Timeout = 1f;

        public void Construct()
        {
            Http.Init(new UnityHttpService());
        }

        public void GetTimeNow()
        {
            Http.Get(string.Concat(DefaultServer, "/api/user/now")).Send();
        }

        public void CreateUser()
        {
            var dic = new Dictionary<string, string>();
            dic.Add("authKey", "testKey2");
            dic.Add("nickname", "nickname2");
            var req = UnityWebRequest.Post(string.Concat(DefaultServer, Endpoint.UserCreate), dic);
            req.Send();
        }

        public async void ExecuteRequest(HttpRequestEvent httpRequestEvent)
        {
            UnityWebRequest request = null;

            var uri = string.Concat(DefaultServer, httpRequestEvent.Endpoint);

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
            ExecuteAsync(operation, httpRequestEvent);
        }

        private async UniTask ExecuteAsync(UnityWebRequestAsyncOperation operation, HttpRequestEvent httpRequestEvent)
        {
            var currentResult = operation.webRequest.result;
            var currentProgress = 0f;
            var webRequest = operation.webRequest;
            var timer = Timeout;
            // Log.Warn(Enum.GetName(typeof(UnityWebRequest.Result), currentResult));

            _executingRequests.Add(httpRequestEvent);

            while (!operation.isDone)
            {
                if (currentResult != webRequest.result)
                {
                    currentResult = webRequest.result;
                    // Log.Warn(Enum.GetName(typeof(UnityWebRequest.Result), currentResult));
                }

                if (currentProgress < operation.progress)
                {
                    currentProgress = operation.progress;
                    httpRequestEvent.ProgressChanged?.Invoke(currentProgress);
                }

                await UniTask.NextFrame();
                timer -= Time.deltaTime;

                if (timer <= 0f)
                {
                    HttpErrorTimeoutLog(httpRequestEvent);
                    httpRequestEvent.Timeout?.Invoke(httpRequestEvent);
                    return;
                }
            }

            httpRequestEvent.Response?.Invoke(webRequest.downloadHandler);

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.InProgress:
                    Log.ErrorUnknown();
                    break;
                case UnityWebRequest.Result.Success:
                    httpRequestEvent.Success?.Invoke(webRequest.downloadHandler);
                    SetServerStatus(true);
                    break;
                case UnityWebRequest.Result.ConnectionError:
                    HttpErrorLog(httpRequestEvent, webRequest);
                    httpRequestEvent.Error?.Invoke(httpRequestEvent, webRequest);
                    SetServerStatus(false);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                case UnityWebRequest.Result.DataProcessingError:
                    HttpErrorLog(httpRequestEvent, webRequest);
                    httpRequestEvent.Error?.Invoke(httpRequestEvent, webRequest);
                    SetServerStatus(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _executingRequests.Remove(httpRequestEvent);
        }

        private void HttpErrorLog(HttpRequestEvent httpRequestEvent, UnityWebRequest webRequest)
        {
            Log.Error($"Error on HttpRequestEvent:\"{httpRequestEvent.Endpoint}\" {Environment.NewLine}" +
                      $"Error message: {webRequest.error}");
        }

        private void HttpErrorTimeoutLog(HttpRequestEvent httpRequestEvent)
        {
            Log.Error($"Timeout on HttpRequestEvent:\"{httpRequestEvent.Endpoint}\"");
        }


        private void SetServerStatus(bool isOnline)
        {
            if (_isOnline != isOnline)
            {
                new ServerStatusChangedEvent(isOnline).Fire();
            }

            _isOnline = isOnline;
        }
    }
}