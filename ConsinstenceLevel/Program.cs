using Microsoft.Azure.Cosmos;
using Shareds;
using Shareds.Models;

CosmosClient client = CosmosClientSingleton.Create();

Container container = client.GetContainer("cosmicworks", "products");

string id = "c665d431-29bb-4301-9b62-4d4237f60030";
string categoryId = "Toys";
PartitionKey partitionKey = new(categoryId);

ItemResponse<Product> response = await container.ReadItemAsync<Product>(id, partitionKey);

Console.WriteLine($"STRONG Request Charge:\t{response.RequestCharge:0.00} RUs");

Console.WriteLine("Type the keyboard to continue...");
Console.ReadLine();

ItemRequestOptions options = new()
{
    ConsistencyLevel = ConsistencyLevel.Eventual
};

response = await container.ReadItemAsync<Product>(id, partitionKey, requestOptions: options);

Console.WriteLine($"EVENTUAL Request Charge:\t{response.RequestCharge:0.00} RUs");

Console.WriteLine("Type the keyboard to exit...");
Console.ReadLine();