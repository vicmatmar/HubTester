using System;
using Centralite.Database;

namespace Centralite.Common.Models
{
    public class Label
    {
        // Constant Values that signify the offset a value should be when encoding label data to Lowes Specifications
        private const int StationOffset = 10;
        private const int SiteOffset = 17;
        private const int DateYearOffset = 22;
        private const int DateDayOffset = 27;
        private const int DateMonthOffset = 32;
        private const int HWRevisionOffset = 36;
        private const int ModelEncodingNumberOffset = 39;

        private const int InvalidModelEncodingNumber = -1;

        private int CentraliteProductionSite = 2;
        private int CentraliteSmtProductionSite = 6;
        private int JabilProductionSite = 3;
        private int KeytronicsProductionSite = 5;

        private const string TestSku = "9999-ABCD";
        private const string UsaProductionSite = "USA";
        private const string MexicoProductionSite = "MEXICO";

        public Label(int serialNum, byte[] eui, int tester, DateTime date, int station, int site, ushort firmwareVersion, Product product, int hwRevision, int bomRevision)
        {
            SerialNumber = serialNum;
            EUI = eui;
            Tester = tester;
            Date = date;
            Station = station;
            Site = site;
            FirmwareVersion = firmwareVersion;
            Product = product;
            HardwareVersion = hwRevision;
            BomRevision = bomRevision;
        }

        // Encodes the label information to the required specifications of Lowes and returns the encoded string
        public string EncodeLowesManufacturingInformation()
        {
            int modelEncodingNumber = 0;
            ulong encodedLabelNumber = 0;
            // Check to see if this is a test label
            if (!this.Product.SKU.Equals(TestSku))
            {
                modelEncodingNumber = (int)Product.ModelEncodingNumber;

                if (modelEncodingNumber == InvalidModelEncodingNumber)
                {
                    return string.Empty;
                }
            }

            // Takes the information of the label and encodes it into the required Lowes format
            encodedLabelNumber |= ((ulong)this.Tester);
            encodedLabelNumber |= ((ulong)this.Station << StationOffset);
            encodedLabelNumber |= ((ulong)this.Site << SiteOffset);
            // Get the last two digits of the year (2015 = 15)
            encodedLabelNumber |= ((ulong)(this.Date.Year % 100) << DateYearOffset);
            encodedLabelNumber |= ((ulong)this.Date.Day << DateDayOffset);
            // Month encoding starts at 0 (Jan)
            encodedLabelNumber |= ((ulong)(this.Date.Month - 1) << DateMonthOffset);
            // Board Revision is being passed in as a character, and therefore needs to be converted back to an int
            encodedLabelNumber |= ((ulong)(BoardRevision.CharToRevision((char)this.HardwareVersion)) << HWRevisionOffset);
            encodedLabelNumber |= ((ulong)modelEncodingNumber << ModelEncodingNumberOffset);


            byte[] encodedLabelNumberBytesArray = BitConverter.GetBytes(encodedLabelNumber);

            // Put the encodedLabelNumberBytesArray into a big endian format
            Array.Reverse(encodedLabelNumberBytesArray);

            // Array for final encoded information
            byte[] euiEncodedLabelNumber = new byte[encodedLabelNumberBytesArray.Length + EUI.Length];

            // Concatenate the two arrays together
            EUI.CopyTo(euiEncodedLabelNumber, 0);
            encodedLabelNumberBytesArray.CopyTo(euiEncodedLabelNumber, EUI.Length);

            // Convert the Byte array into a string of hex digits
            return (BitConverter.ToString(euiEncodedLabelNumber).Replace("-", ""));
        }

        public int SerialNumber { get; private set; }
        public byte[] EUI { get; private set; }
        public int Tester { get; private set; }
        public DateTime Date { get; private set; }
        public int Station { get; private set; }
        public int Site { get; private set; }
        public ushort FirmwareVersion { get; private set; }
        public int HardwareVersion { get; private set; }
        public int BomRevision { get; private set; }
        public Product Product { get; private set; }

        public string FormattedEui
        {
            get
            {
                return BitConverter.ToString(EUI).Replace("-", ":");
            }
        }

        public string FormattedShortEui
        {
            get
            {
                return BitConverter.ToString(EUI).Replace("-", "");
            }
        }

        public string FormattedSerialNumber
        {
            get
            {
                return string.Format("{0}{1:000000000}", Product.SerialNumberCode, SerialNumber);
            }
        }

        public string InformationLine
        {
            get
            {
                return string.Format("{0:00} {1:MMddyy} {2:00} {3:00} {4:X4} {5:00}", Tester, Date, Station, Site, FirmwareVersion, "B" + BomRevision);
            }
        }

        public string ProductionCountry
        {
            get
            {
                if (this.Site == CentraliteProductionSite || this.Site == CentraliteSmtProductionSite)
                {
                    return UsaProductionSite;
                }
                else if (this.Site == JabilProductionSite || this.Site == KeytronicsProductionSite)
                {
                    return MexicoProductionSite;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
