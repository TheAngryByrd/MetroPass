using System;

namespace MetroPass.UI.Tests.Mocks
{
    public class MockEventAggregator : Caliburn.Micro.IEventAggregator
    {
        public object Message { get; set; }

        public bool HandlerExistsFor(Type messageType)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public void Subscribe(object instance)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public void Unsubscribe(object instance)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public void Publish(object message)
        {
            // TODO: Implement this method
            Message = message;
        }

        public void Publish(object message, Action<Action> marshal)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public Action<Action> PublicationThreadMarshaller
        {
            get
            {
                // TODO: Implement this property getter
                throw new NotImplementedException();
            }
            set
            {
                // TODO: Implement this property setter
                throw new NotImplementedException();
            }
        }
    }
}