using sm_application.Scripts.Main.Events;

namespace Events
{
    public class CheckUserExistRequiredEvent : BaseEvent
    {
        public string UniqueDeviceId;
    }
}