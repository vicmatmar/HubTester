using Centralite.Database;
using System.Linq;
using System.Text.RegularExpressions;

namespace MacUtility
{
    public static class MacAddressGenerator
    {
        public static string Generate(long startAddress, long endAddress)
        {
            using (var dbContext = new ManufacturingStoreEntities())
            {
                var mac = dbContext.GetNextMac(startAddress, endAddress);
                return LongToStr(mac);
            }
        }

        public static string LongToStr(long lmac)
        {
            return lmac.ToString("X12");
        }

        /// <summary>
        /// Extract mac address from string
        /// </summary>
        /// <param name="macstr"></param>
        /// <returns>null of the matching portion of a mac string</returns>
        public static string ExtractMacString(string macstr)
        {
            if (string.IsNullOrEmpty(macstr))
                return null;

            // We check for 2 possible formats: 1E2355ffAA33 or 1E:23:55:ff:AA:33
            Regex regex = new Regex(@"[0-9,a-f,A-f]{12}");
            Match match = regex.Match(macstr);
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                regex = new Regex(@"([0-9,a-f,A-f]{2}:){5}[0-9,a-f,A-f]{2}");
                match = regex.Match(macstr);
                if (match.Success)
                {
                    return match.Value;
                }
            }

            return null;
        }
    }
}
