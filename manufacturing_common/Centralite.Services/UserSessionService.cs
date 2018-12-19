using Centralite.Common.Interfaces;
using Centralite.Common.Models;
using Centralite.Database;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Centralite.Services
{
    [Export(typeof(IUserSessionService))]
    public class UserSessionService : IUserSessionService
    {
        private const string RESULT_PASSED = "Passed";
        private const string OTA_ISA_NAME = "OTA-SPOOF";

        private IDataContextFactory dataContextFactory;
        private IErrorProducerService errorProducerService;

        private UserSession userSession;
        public UserSession UserSession
        {
            get
            {
                return userSession;
            }
        }

        [ImportingConstructor]
        public UserSessionService(IDataContextFactory dataContextFactory, IErrorProducerService errorProducerService)
        {
            this.dataContextFactory = dataContextFactory;
            this.errorProducerService = errorProducerService;
        }

        public void CreateUserSession(int testerId)
        {
            using (var manufacturingStoreRepository = dataContextFactory.CreateManufacturingStoreRepository())
            {
                // Start User Session with tester information
                userSession = new UserSession()
                {
                    ApplicationName = "Home Automation Tester",
                    TesterId = testerId,
                    StartTime = DateTime.Now
                };

                manufacturingStoreRepository.UserSessions.Add(userSession);
                manufacturingStoreRepository.SaveChanges();
            }
        }

        public void CloseUserSession()
        {
            // End User Session for tester information
            using (var manufacturingStoreRepository = dataContextFactory.CreateManufacturingStoreRepository())
            {
                var userSession = manufacturingStoreRepository.UserSessions.Where(session => session.Id == UserSession.Id).FirstOrDefault();
                if (userSession != null)
                {
                    userSession.EndTime = DateTime.Now;
                    manufacturingStoreRepository.SaveChanges();
                }
            }
        }

        public void SaveOtaUpgrade(int ProductId, int FirmwareId, string Eui)
        {
            if (UserSession == null)
            {
                errorProducerService.AddMessage(new ErrorMessage("No User Session Open", ErrorType.Exception));
            }

            using (var manufacturingStoreRepository = dataContextFactory.CreateManufacturingStoreRepository())
            {
                var product = manufacturingStoreRepository.Products.AsNoTracking().FirstOrDefault(prod => prod.Id == ProductId);

                if (product == null)
                {
                    errorProducerService.AddMessage(new ErrorMessage(string.Format("No Product found with Id {0}", ProductId), ErrorType.Exception));
                    return;
                }

                var boardRevision = product.Board.BoardRevisions.OrderByDescending(br => br.Revision).FirstOrDefault();

                if (boardRevision == null)
                {
                    errorProducerService.AddMessage(new ErrorMessage(string.Format("No Board Revisions found for Product Id {0}", ProductId), ErrorType.Exception));
                    return;
                }

                var result = manufacturingStoreRepository.Results.AsNoTracking().FirstOrDefault(res => res.Text == RESULT_PASSED);

                if (result == null)
                {
                    errorProducerService.AddMessage(new ErrorMessage("Passed Result not found, something is misconfigured", ErrorType.Exception));
                }

                // Create Test Session
                var testSession = new TestSession();
                testSession.ProductId = ProductId;
                testSession.BoardRevisionId = boardRevision.Id;
                testSession.FirmwareId = FirmwareId;
                testSession.UserSessionId = UserSession.Id;
                testSession.ResultId = result.Id;
                testSession.StartTime = DateTime.Now;
                manufacturingStoreRepository.TestSessions.Add(testSession);
                manufacturingStoreRepository.SaveChanges();

                var euiList = manufacturingStoreRepository.EuiLists.AsNoTracking().FirstOrDefault(eui => eui.EUI == Eui);

                if (euiList == null)
                {
                    errorProducerService.AddMessage(new ErrorMessage(string.Format("EUI {0} not found in database for OTAed device", Eui), ErrorType.Exception));
                }

                var isa = manufacturingStoreRepository.InsightAdapters.AsNoTracking().FirstOrDefault(insight => insight.Name == OTA_ISA_NAME);

                if (isa == null)
                {
                    errorProducerService.AddMessage(new ErrorMessage("OTA Spoof ISA not found. Something is misconfigured", ErrorType.Exception));
                }

                // Create Target Device
                var targetDevice = new TargetDevice();
                targetDevice.IsaId = isa.Id;
                targetDevice.EuiId = euiList.Id;
                targetDevice.TestSessionId = testSession.Id;
                targetDevice.ResultId = result.Id;
                manufacturingStoreRepository.TargetDevices.Add(targetDevice);

                testSession.EndTime = DateTime.Now;

                manufacturingStoreRepository.SaveChanges();
            }
        }
    }
}
