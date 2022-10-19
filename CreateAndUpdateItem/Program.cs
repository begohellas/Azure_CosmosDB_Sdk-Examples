using Microsoft.Azure.Cosmos;
using Shareds;
using Shareds.Models;
using System.Net;

CosmosClient client = CosmosClientSingleton.Create();

Database database = await client.CreateDatabaseIfNotExistsAsync("cosmicworks");
Container container = await database.CreateContainerIfNotExistsAsync(id: "products", partitionKeyPath: "/categoryId", throughput: 400);

string id = "4df541fe-ce3c-4c7f-b8bd-77a9a9141a52";
string categoryId = "Toys";

#region CREATE_ITEM
Product saddle = new()
{
    Id = id,
    CategoryId = categoryId,
    Name = "Barbie",
    Price = 45.99d,
    Tags = new string[]
    {
         "new",
         "eccomerce"
    }
};

ItemResponse<Product> responseCreate;

try
{
    responseCreate = await container.CreateItemAsync<Product>(saddle, new PartitionKey(saddle.CategoryId));
}
catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
{
    await Console.Out.WriteLineAsync($"product {saddle.Id} already exists");
}
#endregion

#region RETRIEVE_ITEM
PartitionKey partitionKey = new(saddle.CategoryId);

saddle = await container.ReadItemAsync<Product>(id, partitionKey);

await Console.Out.WriteLineAsync($"Item Retrieved: [{saddle.Id}]\tName: {saddle.Name} (Price: {saddle.Price:C})");
#endregion

#region UPSER_TITEM
saddle.Price = 32.55d;
saddle.Name = "Barbie & Ken";

await container.UpsertItemAsync<Product>(saddle);
#endregion

#region RETRIEVE_AND_UPSERTITEM_WITH_CONCURRENCYCONTROL
Console.WriteLine("### ConcurrencyControl ###");
ItemResponse<Product> response = await container.ReadItemAsync<Product>(id, partitionKey);
Product product = response.Resource;
await Console.Out.WriteLineAsync($"Existing ETag:\t{response.ETag}");

product.Price = 50d;

ItemRequestOptions requestOptions = new() { IfMatchEtag = response.ETag };
response = await container.UpsertItemAsync<Product>(product, partitionKey, requestOptions: requestOptions);
await Console.Out.WriteLineAsync($"New ETag:\t{response.ETag}");

product.Price = 52d;

try
{
    response = await container.UpsertItemAsync(response.Resource, requestOptions: requestOptions);
}
catch (Exception ex)
{
    await Console.Out.WriteLineAsync($"Update error:\t{ex.Message}");
}

#endregion

Console.WriteLine("Type the keyboard to exit...");
Console.ReadLine();