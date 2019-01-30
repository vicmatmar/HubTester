using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Centralite.Database;
using System.Linq;

namespace HubTester
{
    public class DataUtils
    {
        /// <summary>
        /// Gets the last hub with corresponding EUI string
        /// </summary>
        /// <param name="EUI"></param>
        /// <returns></returns>
        public static JiliaHub GetHub(string EUI)
        {
            using (var dc = new ManufacturingStoreEntities())
            {
                // first get eui_id
                int eui_id = dc.EuiLists.Where(e => e.EUI == EUI).Single().Id;

                // Get hub for this eui
                return dc.JiliaHubs.Where(h => h.EuiId == eui_id).OrderByDescending(h => h.Id).First();
            }
        }

        /// <summary>
        /// Gets the mac object from db with corresponding mac string
        /// </summary>
        /// <param name="mac"></param>
        /// <returns></returns>
        public static MacAddress GetMacAddress(string mac)
        {
            string macstr = mac.Replace(":", "").Replace("-", "").Replace("=", "").Trim();
            long lmac = long.Parse(macstr);

            using (var dbContext = new ManufacturingStoreEntities())
            {
                return dbContext.MacAddresses.Where(m => m.MAC == lmac).Single();
            }
        }

        public static EuiList GetEUI(string eui)
        {
            using (var dbContext = new ManufacturingStoreEntities())
            {
                return dbContext.EuiLists.Where(e => e.EUI == eui).Single();
            }
        }

    }
}
