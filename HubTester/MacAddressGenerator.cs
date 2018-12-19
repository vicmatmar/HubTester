using Centralite.Database;

namespace MacUtility
{
    public static class MacAddressGenerator
    {
        public static string Generate(long startAddress, long endAddress)
        {
            using (var dbContext = new ManufacturingStoreEntities())
            {
                var mac = dbContext.GetNextMac(startAddress, endAddress);

                return mac.ToString("X12");
            }
        }
    }
}
