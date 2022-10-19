using Microsoft.Azure.Cosmos;
using Shareds;

List<string> regions = new()
{
    Regions.WestEurope,
    Regions.NorthEurope,
    Regions.EastUS
};

CosmosClientOptions options = new()
{
    ApplicationPreferredRegions = regions
};

CosmosClient client = CosmosClientSingleton.Create(options);
Container container = client.GetContainer("cosmicworks", "products");

string id = "951db92e-9b0d-41e7-b969-ac479f8a5aee"; //insert one id take from products container
string categoryId = "Toys";
PartitionKey partitionKey = new(categoryId);

ItemResponse<dynamic> response = await container.ReadItemAsync<dynamic>(id, partitionKey);

Console.WriteLine($"Item Id:\t{response.Resource.id}");
Console.WriteLine("-- Response Diagnostics JSON --");
Console.WriteLine($"{response.Diagnostics}");

Console.WriteLine();
Console.WriteLine("Type the keyboard to exit...");
Console.ReadLine();