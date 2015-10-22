using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;

namespace ConfigurationService.Persistence
{
    public class StatePersister : IStatePersister
    {
        private Logger log = LogManager.GetCurrentClassLogger();
        private const string fileName = "configurations";
        private const string tempFileName = fileName + ".temp";
        private const string configurationsFileName = fileName + ".json";

        public void Persist(List<Configuration> transactions)
        {
            try
            {
                //Todo check if it is smart to save it... if the same just return
                log.Debug("Persisting state {0}");
                //We do this in 3 step :
                //0 we write a temp state file
                if (File.Exists(tempFileName))
                {
                    log.Warn("We still have the temp file, something did not ended well last time we tried to save this project {0}", fileName);
                    File.Delete(tempFileName);
                    //We could try to see, if there is no state file, if this one is correct and continue the process.
                }
                File.AppendAllText(tempFileName, JsonConvert.SerializeObject(transactions));
                //1 we remove the current state if exist
                if (File.Exists(configurationsFileName))
                {
                    Clean();
                }
                //2 we rename the first state file
                File.Move(tempFileName, configurationsFileName);

            }
            catch (Exception e)
            {
                log.Error(string.Format("Error while persisting state for configurations."));
                log.Error(e);
                throw;
            }
        }


        public List<Configuration> Read()
        {
            var result = new List<Configuration>();
            try
            {
                log.Debug("Reading configurations.");
                if (File.Exists(configurationsFileName))
                {
                    var stringState = File.ReadAllText(configurationsFileName);
                    var temp = JsonConvert.DeserializeObject<List<Configuration>>(stringState);
                    temp.ForEach(result.Add);
                }
                else
                {
                    log.Debug("No file found");
                }
                log.Debug("Found {0} configurations", result.Count);
            }
            catch (Exception e)
            {
                log.Error(string.Format("Error while Reading state for configurations"));
                log.Error(e);
                throw;
            }
            return result;
        }

        public void Clean()
        {
            log.Info("Deleting the configuration file");
            File.Delete(configurationsFileName);
        }
    }
}
