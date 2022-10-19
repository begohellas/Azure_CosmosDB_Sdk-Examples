using Microsoft.Azure.Cosmos;
using Shareds;
using Shareds.Models;

CosmosClient client = CosmosClientSingleton.Create();

Database database = await client.CreateDatabaseIfNotExistsAsync("cosmicworks");
Container container = await database.CreateContainerIfNotExistsAsync(id: "products", partitionKeyPath: "/categoryId", throughput: 400);

Product saddle = new()
{
    Id = Guid.NewGuid().ToString(),
    CategoryId = "Sports",
    Name = "Road Saddle",
    Price = 55.99d,
    Tags = new string[]
    {
         "tan",
         "new",
         "crisp"
    }
};

Product handlebar = new()
{
    Id = Guid.NewGuid().ToString(),
    CategoryId = "Sports",
    Name = "Rusty Handlebar",
    Price = 105.10d,
    Tags = new string[]
    {
        "tan",
        "new",
        "crisp"
    }
};

PartitionKey partitionKey = new("Sports");
TransactionalBatch batch = container.CreateTransactionalBatch(partitionKey)
    .CreateItem<Product>(saddle)
    .CreateItem<Product>(handlebar);

using TransactionalBatchResponse response = await batch.ExecuteAsync();

Console.WriteLine($"esito batch: {response.IsSuccessStatusCode}\n");
Console.WriteLine($"status code: {response.StatusCode}");

Console.WriteLine("Type the keyboard to exit...");
Console.ReadLine();