using ZXing;

namespace Centralite.BradyPrinter
{
    public static class ElementTypes
    {
        public const string HEADER = "Header";
        public const string BARCODE = "Barcode";
        public const string TEXT = "Text";
    }

    public static class BarcodeTypes
    {
        public const string CODE_128 = "Code128";
        public const string CODE_39 = "Code39";
        public const string QR_CODE = "QRCode";

        public static BarcodeFormat GetBarcodeType(string barcodeType)
        {
            BarcodeFormat barcodeFormat = BarcodeFormat.CODE_128;

            switch (barcodeType)
            {
                case CODE_128:
                    barcodeFormat = BarcodeFormat.CODE_128;
                    break;
                case CODE_39:
                    barcodeFormat = BarcodeFormat.CODE_39;
                    break;
                case QR_CODE:
                    barcodeFormat = BarcodeFormat.QR_CODE;
                    break;
            }

            return barcodeFormat;
        }
    }
}
