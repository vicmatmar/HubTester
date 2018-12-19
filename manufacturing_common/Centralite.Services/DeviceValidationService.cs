using Centralite.Common.Interfaces;
using System;
using Centralite.Common.Models;
using Centralite.Ember.Ezsp;
using Centralite.Database;
using System.ComponentModel.Composition;
using Centralite.Ember.DataTypes;
using Centralite.Common.Utilities;
using System.Linq;

namespace Centralite.Services
{
    [Export(typeof(IDeviceValidationService))]
    public class DeviceValidationService : IDeviceValidationService
    {
        private const string EM250_CHIP = "EM250";
        IEzspService ezspService;
        IDataContextFactory dataContextFactory;
        IErrorProducerService errorProducerService;

        public Product ValidProduct { get; set; }
        public StationSite CurrentStationSite { get; set; }

        [ImportingConstructor]
        public DeviceValidationService(IEzspService ezspService, IDataContextFactory dataContextFactory, IErrorProducerService errorProducerService)
        {
            this.ezspService = ezspService;
            this.dataContextFactory = dataContextFactory;
            this.errorProducerService = errorProducerService;
        }

        public void ValidateDevice(ZigbeeDeviceBase device, EzspIncomingMessageHandlerResponse message)
        {
            if (message != null)
            {
                EmberEui64 eui;
                string modelString;
                uint firmwareVersion;

                if ((ZdoMessageParser.TryParseDeviceAnnounce(message, out eui) || ZdoMessageParser.TryParseIEEEAddressResponse(message, out eui)))
                {
                    if (VerifyEui(eui))
                    {
                        device.Eui = eui;
                        OnAddDevice?.Invoke(device);
                    }
                    else
                    {
                        OnRemoveDevice?.Invoke(device);
                    }
                }
                else if (HaMessageParser.TryParseModelString(message, out modelString))
                {
                    if (VerifyModelString(modelString))
                    {
                        device.ModelString = modelString;
                    }
                    else
                    {
                        OnRemoveDevice?.Invoke(device);
                    }
                }
                else if (HaMessageParser.TryParseFirmwareVersion(message, out firmwareVersion))
                {
                    device.LongFirmwareVersion = (firmwareVersion == default(uint)) ? uint.MaxValue : firmwareVersion;
                }
            }

            RequestDeviceInformation(device);
        }

        private void RequestDeviceInformation(ZigbeeDeviceBase device)
        {
            if (device.Eui == null)
            {
                ZdoMessageComposer.SendIEEEAddressRequest(device.Address, ezspService);
            }
            else if (device.ModelString == default(string))
            {
                // Send ModelString request
                HaMessageComposer.SendModelStringRequest(device.Address, ezspService);
            }
            else if (device.LongFirmwareVersion == default(uint))
            {
                // Send Firmware Version Request
                HaMessageComposer.SendFirmwareVersionRequest(device.Address, ezspService);
            }
            else
            {
                device.Validated = true;
            }
        }

        private bool VerifyEui(EmberEui64 eui)
        {
            bool result = false;

            using (var manufacturingStoreRepository = dataContextFactory.CreateManufacturingStoreRepository())
            {
                var euiString = eui.ToString();
                var dbEui = manufacturingStoreRepository.EuiLists.AsNoTracking().FirstOrDefault(e => e.EUI == euiString);

                string dbEuiSKU = null;
                if (dbEui != null)
                {
                    dbEuiSKU = dbEui.TargetDevices.OrderByDescending(x => x.Id).FirstOrDefault()?.TestSession?.Product?.SKU;

                    if (dbEuiSKU == ValidProduct?.SKU)
                    {
                        result = true;
                    }
                    else
                    {
                        errorProducerService.AddMessage(new ErrorMessage(string.Format("SKU: {0} is invalid for the selected product", dbEuiSKU), ErrorType.Error));
                    }
                }
                else
                {
                    if (ValidProduct?.Board.ChipType.Name == EM250_CHIP)
                    {
                        // EM250 Chips do not have EUIs in DB since they are not coded in EBL
                        dbEui = new EuiList()
                        {
                            EUI = euiString,
                            ProductionSiteId = CurrentStationSite.ProductionSite.Id
                        };

                        manufacturingStoreRepository.EuiLists.Add(dbEui);
                        manufacturingStoreRepository.SaveChanges();

                        result = true;
                    }
                    else
                    {
                        errorProducerService.AddMessage(new ErrorMessage(string.Format("EUI: {0} could not be found in database", euiString), ErrorType.Error));
                    }
                }

                return result;
            }
        }

        private bool VerifyModelString(string modelString)
        {
            bool result = false;

            if (modelString == ValidProduct?.ModelString)
            {
                result = true;
            }
            else
            {
                errorProducerService.AddMessage(new ErrorMessage(string.Format("Modelstring mismatch for device joining. {0} should be {1}", modelString, ValidProduct?.ModelString), ErrorType.Error));
            }

            return result;
        }

        /// <summary>
        /// Event to subscribe to for when a device is ready to be added to DeviceList
        /// </summary>
        public event Action<ZigbeeDeviceBase> OnAddDevice;

        /// <summary>
        /// Event to subscribe to for when a device should be removed from DeviceList
        /// </summary>
        public event Action<ZigbeeDeviceBase> OnRemoveDevice;
    }
}
