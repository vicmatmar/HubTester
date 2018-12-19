namespace Centralite.Common.Interfaces
{
    public interface IDataContextFactory
    {
        IManufacturingStoreRepository CreateManufacturingStoreRepository();
    }
}
