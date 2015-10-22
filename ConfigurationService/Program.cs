using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationService.Persistence;
using NLog;

namespace ConfigurationService
{
    class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            logger.Info("Starting application");
        }
    }
}
