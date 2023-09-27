using sm_application.Events;

namespace Events
{
    public class CheckUserExistRequiredEvent : BaseEvent
    {
        public string UniqueDeviceId;
    }
}