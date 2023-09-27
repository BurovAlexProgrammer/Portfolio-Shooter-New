using sm_application.Events;

namespace Events
{
    public class ServerStatusChangedEvent : BaseEvent
    {
        public readonly bool IsOnline;

        public ServerStatusChangedEvent(bool isOnline)
        {
            IsOnline = isOnline;
        }
    }
}