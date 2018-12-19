using System.Globalization;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps;
using ZXing;
using ZXing.Common;

namespace Centralite.BradyPrinter
{
    public class BradyPrinter
    {
        private const int HEADER_ELEMENTS_COUNT = 6;
        private const int BARCODE_ELEMENTS_COUNT = 7;
        private const int TEXT_ELEMENTS_COUNT = 6;

        private const int BARCODE_WIDTH = 1200;
        private const int BARCODE_HEIGHT = 1800;

        private const string CENTER_COMMAND = "Center";

        private PrintTicket PrintTicket;
        private DrawingVisual DrawingVisual;
        private XpsDocumentWriter DocumentWriter;

        private int TotalWidth;
        private int LabelCount;
        private int LabelWidth;
        private int LabelHeight;
        private int LabelSpacing;

        private int OffSetValue
        {
            get
            {
                return (LabelWidth + LabelSpacing - 5);
            }
        }

        public BradyPrinter()
        {
            var defaultPrintQueue = LocalPrintServer.GetDefaultPrintQueue();

            DocumentWriter = PrintQueue.CreateXpsDocumentWriter(defaultPrintQueue);
            PrintTicket = defaultPrintQueue.DefaultPrintTicket;
            DrawingVisual = new DrawingVisual();
        }

        public void LoadLabel(string fileContents)
        {
            var lines = fileContents.Split('\n');

            var drawingContext = DrawingVisual.RenderOpen();

            foreach (var line in lines)
            {
                ParseLine(line, drawingContext);
            }

            drawingContext.Close();
        }

        public void PrintLabel()
        {
            DocumentWriter.Write(DrawingVisual, PrintTicket);
        }

        private void ParseLine(string line, DrawingContext drawingContext)
        {
            var elements = line.Split(',');

            if (elements.Length > 0)
            {
                switch (elements[0])
                {
                    case ElementTypes.HEADER:
                        HandleHeaderLine(elements);
                        break;
                    case ElementTypes.BARCODE:
                        HandleBarcodeLine(elements, drawingContext);
                        break;
                    case ElementTypes.TEXT:
                        HandleTextLine(elements, drawingContext);
                        break;
                }
            }
        }

        private void HandleHeaderLine(string[] elements)
        {
            if (elements.Length == HEADER_ELEMENTS_COUNT)
            {
                int.TryParse(elements[1], out TotalWidth);
                int.TryParse(elements[2], out LabelCount);
                int.TryParse(elements[3], out LabelWidth);
                int.TryParse(elements[4], out LabelHeight);
                int.TryParse(elements[5], out LabelSpacing);

                PrintTicket.PageMediaSize = new PageMediaSize(TotalWidth, LabelHeight);
            }
        }

        private void HandleBarcodeLine(string[] elements, DrawingContext drawingContext)
        {
            if (elements.Length == BARCODE_ELEMENTS_COUNT)
            {
                string barcodeType = elements[1];

                double x1;
                double y1;
                double.TryParse(elements[2], out x1);
                double.TryParse(elements[3], out y1);

                double x2;
                double y2;
                double.TryParse(elements[4], out x2);
                double.TryParse(elements[5], out y2);

                string barcodeContents = elements[6];

                for (int i = 0; i < LabelCount; i++)
                {
                    int offset = i * (OffSetValue);

                    var bitMapImage = GenerateBarcodeImage(barcodeContents, BarcodeTypes.GetBarcodeType(barcodeType));
                    var barcodeRectangle = new Rect(new Point(offset + x1, y1), new Point(offset + x2, y2));

                    drawingContext.DrawImage(bitMapImage, barcodeRectangle);
                }
            }
        }

        private void HandleTextLine(string[] elements, DrawingContext drawingContext)
        {
            if (elements.Length == TEXT_ELEMENTS_COUNT)
            {
                string textContents = elements[5];
                string fontName = elements[3];

                int fontSize;
                int.TryParse(elements[4], out fontSize);

                var formattedText = new FormattedText(textContents, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface(fontName), fontSize, Brushes.Black);

                double x;
                if (elements[1] == CENTER_COMMAND)
                {
                    x = (LabelWidth - formattedText.Width) / 2;
                }
                else
                {
                    double.TryParse(elements[1], out x);
                }

                double y;
                if (elements[2] == CENTER_COMMAND)
                {
                    y = (LabelHeight - formattedText.Height) / 2;
                }
                else
                {
                    double.TryParse(elements[2], out y);
                }

                for (int i = 0; i < LabelCount; i++)
                {
                    int offset = i * (OffSetValue);

                    drawingContext.DrawText(formattedText, new Point(offset + x, y));
                }
            }
        }

        private BitmapImage GenerateBarcodeImage(string barcodeContent, BarcodeFormat barcodeType)
        {
            var barcodeWriter = new ZXing.Presentation.BarcodeWriter();
            barcodeWriter.Format = barcodeType;
            barcodeWriter.Options = new EncodingOptions
            {
                Width = BARCODE_WIDTH,
                Height = BARCODE_HEIGHT
            };

            var bc = barcodeWriter.Write(barcodeContent);

            return ToBitMapImage(bc);
        }

        private BitmapImage ToBitMapImage(WriteableBitmap writableBitMap)
        {
            var bitMapImage = new BitmapImage();

            using (var stream = new MemoryStream())
            {
                var pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(writableBitMap));
                pngEncoder.Save(stream);
                bitMapImage.BeginInit();
                bitMapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitMapImage.StreamSource = stream;
                bitMapImage.EndInit();
                bitMapImage.Freeze();
            }

            return bitMapImage;
        }
    }
}
