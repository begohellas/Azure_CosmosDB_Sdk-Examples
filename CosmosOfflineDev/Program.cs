using Microsoft.Azure.Cosmos;
using Shareds;

CosmosClient client = CosmosClientSingleton.Create();

Database database = await client.CreateDatabaseIfNotExistsAsync("cosmicworks");
Console.WriteLine($"New Database:\t Id: {database.Id}");

Container container = await database.CreateContainerIfNotExistsAsync("products", "/categoryId", 400);
Console.WriteLine($"New Container:\t Id: {container.Id}");

Console.WriteLine("Type the keyboard to exit...");
Console.ReadLine();
