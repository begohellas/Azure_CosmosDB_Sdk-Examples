using Microsoft.Azure.Cosmos;
using Shareds;
using System.Text.Json;

CosmosClient client = CosmosClientSingleton.Create();

Database database = await client.CreateDatabaseIfNotExistsAsync("cosmicworks");

IndexingPolicy policy = new()
{
    IndexingMode = IndexingMode.Consistent
};
policy.ExcludedPaths.Add(
    new ExcludedPath { Path = "/*" }
);
policy.IncludedPaths.Add(
    new IncludedPath { Path = "/name/?" }
);

ContainerProperties options = new(id: "productsWithCustomIndex", partitionKeyPath: "/categoryId")
{
    IndexingPolicy = policy
};

Console.WriteLine("-- CosmosClientOptions JSON --");
Console.WriteLine($"{JsonSerializer.Serialize(client.ClientOptions, DefaultJsonSerializerOptions.Instance)}");

//create containers with custom indexing policy (view in section Scale & Settings of cosmosDB)
Container container = await database.CreateContainerIfNotExistsAsync(options);
Console.WriteLine($"Container Created [{container.Id}]");

Console.WriteLine("Type the keyboard to exit...");
Console.ReadLine();