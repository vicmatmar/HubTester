using Centralite.Common.Models;

namespace Centralite.Common.Interfaces
{
    public interface IErrorProducerService
    {
        void AddMessage(ErrorMessage errorMessage);
    }
}
