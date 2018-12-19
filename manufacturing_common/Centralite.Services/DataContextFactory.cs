using Centralite.Common.Interfaces;
using System.ComponentModel.Composition;

namespace Centralite.Services
{
    [Export(typeof(IDataContextFactory))]
    public class DataContextFactory : IDataContextFactory
    {
        public IManufacturingStoreRepository CreateManufacturingStoreRepository()
        {
            return new ManufacturingStoreRepository();
        }
    }
}
