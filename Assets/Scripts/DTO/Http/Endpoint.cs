using sm_application.Events;
using sm_application.HttpData;

namespace Game.DTO.Http
{
    public static class Endpoint
    {
        public static HttpEndpointData ServerTime = new HttpEndpointData("/api/user/now", HttpRequestMethod.Get);
        
        public static HttpEndpointData CheckUserUniqueDeviceId = new HttpEndpointData("/api/user/checkUniqueDeviceId", HttpRequestMethod.Post);
        public static HttpEndpointData UserCreate = new HttpEndpointData("/api/user/create", HttpRequestMethod.Post);
    }
}