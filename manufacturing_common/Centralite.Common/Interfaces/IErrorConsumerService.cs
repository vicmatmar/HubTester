using Centralite.Common.Models;
using System;

namespace Centralite.Common.Interfaces
{
    public interface IErrorConsumerService
    {
        ErrorMessage RetrieveMessage();

        event Action ErrorMessageAddedEvent;

        void ClearConsumerEvents();
    }
}
