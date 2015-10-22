using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;

namespace ConfigurationServiceTests
{
    public class BaseTest
    {
        public BaseTest()
        {
            ConsoleTarget target = new ConsoleTarget();
            target.Layout = "${time}|${level:uppercase=true}|${callsite:className=true:includeSourcePath=false:methodName=false}|${message}";

            SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Trace);

            Logger logger = LogManager.GetLogger("ee");
            logger.Debug("another log message");

        }

        [SetUp]
        public virtual void Init()
        {
            //Init for every tests!
            Write("In Init");
        }

        public void Write(string message)
        {
            Debug.WriteLine(message);
        }
        
    }
}
