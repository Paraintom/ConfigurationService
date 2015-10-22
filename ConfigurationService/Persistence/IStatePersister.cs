using System.Collections.Generic;

namespace ConfigurationService.Persistence
{
    public interface IStatePersister
    {
        void Persist(List<Configuration> transactions);
        List<Configuration> Read();
        void Clean();
    }
}
