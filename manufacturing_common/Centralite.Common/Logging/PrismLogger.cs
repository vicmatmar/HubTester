using Centralite.Logging;
using Prism.Logging;
using System.ComponentModel.Composition;

namespace Centralite.Common.Logging
{
    [Export(typeof(ILoggerFacade))]
    public class PrismLogger : ILoggerFacade
    {
        private log4net.ILog logger;

        public PrismLogger()
        {
            logger = Log4NetManager.GetLogger("HomeAutomationTester");
        }

        public void Log(string message, Category category, Priority priority)
        {
            switch (category)
            {
                case Category.Debug:
                    logger.Debug(message);
                    break;
                case Category.Exception:
                    logger.Error(message);
                    break;
                case Category.Info:
                    logger.Info(message);
                    break;
                case Category.Warn:
                    logger.Warn(message);
                    break;
                default:
                    logger.Debug(message);
                    break;
            }
        }
    }
}
