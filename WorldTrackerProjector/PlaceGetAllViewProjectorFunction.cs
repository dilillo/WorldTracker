using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WorldTrackerDomain.Events;
using WorldTrackerDomain.Projectors;
using WorldTrackerProjector.Repositories;

namespace WorldTrackerProjector
{
    public static class PlaceGetAllViewProjectorFunction
    {
        [FunctionName("PlaceGetAllViewProjectorFunction")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "worldtracker",
            collectionName: "events",
            ConnectionStringSetting = "WorldTrackerOptions:CosmosDBConnectionString",
            LeaseCollectionName = "leases",
            LeaseCollectionPrefix = "PlaceGetAllViewProjectorFunction",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, ILogger log, CancellationToken cancellationToken)
        {
            var events = input.Select(i => JsonConvert.DeserializeObject<DomainEventWrapper>(i.ToString()).GetEvent()).ToArray();

            var projector = new PlaceGetAllViewProjector(new ProjectorDomainViewRepository());

            await projector.Project(events, cancellationToken);

            log.LogInformation($"{nameof(PlaceGetAllViewProjectorFunction)} processed {input.Count} documents");
        }
    }
}
