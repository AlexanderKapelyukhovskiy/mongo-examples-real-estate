using System;
using log4net;
using MongoDB.Driver.Core.Events;

namespace RealEstate.App_Start
{
    public class Log4NetMongoEvents : IEventSubscriber
    {
        public static ILog CommodStartedLog = LogManager.GetLogger("CommandStarted");

        private ReflectionEventSubscriber _eventSubscriber;
        public Log4NetMongoEvents()
        {
            _eventSubscriber = new ReflectionEventSubscriber(this);
        }

        public bool TryGetEventHandler<TEvent>(out Action<TEvent> handler)
        {
            return _eventSubscriber.TryGetEventHandler(out handler);
        }

        public void Handle(CommandStartedEvent started)
        {
            CommodStartedLog.Info(new
            {
                started.Command,
                started.CommandName,
                started.ConnectionId,
                started.DatabaseNamespace,
                started.OperationId,
                started.RequestId
            });
        }

        public void Handle(CommandSucceededEvent succeeded)
        {

        }
    }
}