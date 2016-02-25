using Windows.ApplicationModel;

namespace SmartSolar.Device.Messages
{
    public class SuspendStateMessage
    {
        public SuspendStateMessage(SuspendingOperation operation)
        {
            Operation = operation;
        }

        public SuspendingOperation Operation { get; }
    }
}
