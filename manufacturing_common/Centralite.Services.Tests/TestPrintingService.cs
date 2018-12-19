using Centralite.Common.Enumerations;
using Centralite.Common.Interfaces;
using Centralite.Common.Models;
using Centralite.Common.Printers;
using Centralite.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Centralite.Services.Tests
{
    [TestClass]
    public class TestPrintingService
    {
        private PrintingService printingService = new PrintingService();

        [TestMethod]
        public void TestAddLabel()
        {
            Label testLabel = new Label(99, new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 }, 99, DateTime.Now, 99, 99, 99, new Product(), 99, 99);
            printingService.AddLabel(testLabel.SerialNumber, testLabel.EUI, testLabel.Tester, testLabel.Date, testLabel.Station, testLabel.Site, testLabel.FirmwareVersion, testLabel.Product, testLabel.HardwareVersion, testLabel.BomRevision);

            var labels = (List<Label>)typeof(PrintingService).GetField("Labels", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(printingService);
            var printerLabel = labels.First();

            Assert.AreEqual(labels.Count(), 1);
            Assert.AreEqual(printerLabel.SerialNumber, testLabel.SerialNumber);
            Assert.AreEqual(printerLabel.EUI, testLabel.EUI);
            Assert.AreEqual(printerLabel.Tester, testLabel.Tester);
            Assert.AreEqual(printerLabel.Date, testLabel.Date);
            Assert.AreEqual(printerLabel.Station, testLabel.Station);
            Assert.AreEqual(printerLabel.Site, testLabel.Site);
            Assert.AreEqual(printerLabel.FirmwareVersion, testLabel.FirmwareVersion);
            Assert.AreEqual(printerLabel.Product, testLabel.Product);
            Assert.AreEqual(printerLabel.HardwareVersion, testLabel.HardwareVersion);
            Assert.AreEqual(printerLabel.BomRevision, testLabel.BomRevision);
        }

        [TestMethod]
        public void TestPrint()
        {
            Mock<IPrinter> mockPrinter = new Mock<IPrinter>();
            mockPrinter.Setup(x => x.Print(It.IsAny<IEnumerable<Label>>()));

            Label testLabel = new Label(99, new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 }, 99, DateTime.Now, 99, 99, 99, new Product(), 99, 99);
            printingService.AddLabel(testLabel.SerialNumber, testLabel.EUI, testLabel.Tester, testLabel.Date, testLabel.Station, testLabel.Site, testLabel.FirmwareVersion, testLabel.Product, testLabel.HardwareVersion, testLabel.BomRevision);

            var printer = printingService.GetType().GetField("printer", BindingFlags.NonPublic | BindingFlags.Instance);
            printer.SetValue(printingService, mockPrinter.Object);

            var labels = (List<Label>)typeof(PrintingService).GetField("Labels", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(printingService);

            printingService.Print();

            mockPrinter.Verify(x => x.Print(It.IsAny<IEnumerable<Label>>()), Times.AtLeastOnce);
            Assert.AreEqual(labels.Count(), 0);
        }

        [TestMethod]
        public void TestRegisterPrinterType()
        {
            Label testLabel = new Label(99, new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 }, 99, DateTime.Now, 99, 99, 99, new Product(), 99, 99);
            printingService.AddLabel(testLabel.SerialNumber, testLabel.EUI, testLabel.Tester, testLabel.Date, testLabel.Station, testLabel.Site, testLabel.FirmwareVersion, testLabel.Product, testLabel.HardwareVersion, testLabel.BomRevision);

            var labels = (List<Label>)typeof(PrintingService).GetField("Labels", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(printingService);

            printingService.RegisterPrinterType(PrinterType.ZebraPrinter);

            var printer = (IPrinter)typeof(PrintingService).GetField("printer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(printingService);

            Assert.AreEqual(printer.GetType(), typeof(ZebraPrinter));
            Assert.AreEqual(labels.Count(), 0);
        }
    }
}
