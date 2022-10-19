using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ItemListenerTrigger;

public static class ItemListenerTriggerFn
{

    /// <summary>
    /// Lunch functions and change some value in container products
    /// </summary>
    [FunctionName("FnChangeFeedProducts")]
    public static void Run([CosmosDBTrigger(
        databaseName: "cosmicworks",
        collectionName: "products",
        ConnectionStringSetting = "Dp420CosmosEmulator", //save in user secrets
        LeaseCollectionName = "productslease",
        CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input,
        ILogger log)
    {
        if (input?.Count > 0)
        {
            log.LogInformation("Documents modified {Count}", input.Count);
            log.LogInformation("First document {Id}", input[0].Id);
        }
    }
}
