using System;
using System.Collections.Generic;
using Centralite.Common.Interfaces;
using Centralite.Common.Models;
using System.ComponentModel.Composition;

namespace Centralite.Services
{
    [Export(typeof(IErrorProducerService))]
    [Export(typeof(IErrorConsumerService))]
    public class ErrorMessageService : IErrorProducerService, IErrorConsumerService
    {
        private Queue<ErrorMessage> ErrorMessages = new Queue<ErrorMessage>();

        public event Action ErrorMessageAddedEvent;

        public void AddMessage(ErrorMessage errorMessage)
        {
            lock (ErrorMessages)
            {
                ErrorMessages.Enqueue(errorMessage);
            }

            ErrorMessageAddedEvent?.Invoke();
        }

        public void ClearConsumerEvents()
        {
            ErrorMessageAddedEvent = null;
        }

        public ErrorMessage RetrieveMessage()
        {
            lock (ErrorMessages)
            {
                if (ErrorMessages.Count != 0)
                {
                    return ErrorMessages.Dequeue();
                }
                else
                {
                    return default(ErrorMessage);
                }
            }
        }
    }
}
