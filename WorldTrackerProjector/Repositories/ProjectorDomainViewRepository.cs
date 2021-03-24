using System;
using WorldTrackerDomain.Repositories;

namespace WorldTrackerProjector.Repositories
{
    public class ProjectorDomainViewRepository : DomainViewRepository
    {
        public ProjectorDomainViewRepository() : base()
        {
            _connectionString = Environment.GetEnvironmentVariable("WorldTrackerOptions:CosmosDBConnectionString");
        }
    }
}
