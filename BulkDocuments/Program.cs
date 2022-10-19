using Bogus;
using Microsoft.Azure.Cosmos;
using Shareds;
using Shareds.Models;
using System.Text.Json;
using CosmosDatabase = Microsoft.Azure.Cosmos.Database;

CosmosClientOptions options = new()
{
    AllowBulkExecution = true
};

CosmosClient client = CosmosClientSingleton.Create(options);
Console.WriteLine("-- CosmosClientOptions JSON (search section applicationPreferredRegions) --");
Console.WriteLine($"{JsonSerializer.Serialize(client.ClientOptions, DefaultJsonSerializerOptions.Instance)}");

CosmosDatabase database = await client.CreateDatabaseIfNotExistsAsync("cosmicworks");
Container container = await database.CreateContainerIfNotExistsAsync(id: "products", partitionKeyPath: "/categoryId", throughput: 400);

const int amountToInsert = 25_000;
var tags = new string[] { "new", "used", "refurbished" };
var tags2 = new string[] { "ecommerce", "shop" };
List<Product> productsToInsert = new Faker<Product>()
    .StrictMode(true)
    .RuleFor(o => o.Id, _ => Guid.NewGuid().ToString())
    .RuleFor(o => o.Name, f => f.Commerce.ProductName())
    .RuleFor(o => o.Price, f => Convert.ToDouble(f.Commerce.Price(max: 1000, min: 10, decimals: 2)))
    .RuleFor(o => o.CategoryId, f => f.Commerce.Department(1))
    .RuleFor(o => o.Tags, f => new string[] { f.PickRandom(tags), f.PickRandom(tags2) })
    .Generate(amountToInsert);

List<Task> concurrentTasks = new();

foreach (Product product in productsToInsert)
{
    concurrentTasks.Add(container.CreateItemAsync(product, new PartitionKey(product.CategoryId)) // passing the name of the property configured as Partition Key Path in the Container.
        .ContinueWith(itemResponse =>
        {
            if (!itemResponse.IsCompletedSuccessfully)
            {
                AggregateException innerExceptions = itemResponse.Exception.Flatten();
                if (innerExceptions.InnerExceptions.FirstOrDefault(innerEx => innerEx is CosmosException) is CosmosException cosmosException)
                {
                    Console.WriteLine($"Received {cosmosException.StatusCode} - ({cosmosException.Message}).");
                }
                else
                {
                    Console.WriteLine($"Exception {innerExceptions.InnerExceptions.FirstOrDefault()}.");
                }
            }
        }));
}

await Task.WhenAll(concurrentTasks);

Console.WriteLine("Bulk tasks complete");
Console.WriteLine("Type the keyboard to exit...");
Console.ReadLine();