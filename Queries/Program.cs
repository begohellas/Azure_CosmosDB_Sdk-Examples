using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Shareds;
using Shareds.Models;

CosmosClient client = CosmosClientSingleton.Create();
Database database = await client.CreateDatabaseIfNotExistsAsync("cosmicworks");
Container container = await database.CreateContainerIfNotExistsAsync(id: "products", partitionKeyPath: "/categoryId", throughput: 400);

#region query_items_sql
// Query multiple items from container
var query = new QueryDefinition(query: "SELECT * FROM products p");
var queryRequestOptions = new QueryRequestOptions()
{
    MaxItemCount = 100
};

using (FeedIterator<Product> feedIterator = container.GetItemQueryIterator<Product>(
    queryDefinition: query,
    requestOptions: queryRequestOptions))
{
    int pageCount = 0;
    // Iterate query result pages
    while (feedIterator.HasMoreResults)
    {
        FeedResponse<Product> response = await feedIterator.ReadNextAsync();
        Console.Out.WriteLine($"---Page #{++pageCount:0000}");
        // Iterate query results
        foreach (Product item in response)
        {
            await Console.Out.WriteLineAsync($"[Product {item.Id}]\tName: {item.Name,35}\tPrice:{item.Price,15:C}");

        }
    }
}
#endregion

Console.WriteLine("Type the keyboard to view respinse from query with parameters...");
Console.ReadLine();

#region query_items_sql_parameters
var parameterizedQuery = new QueryDefinition(query: "SELECT * FROM products p WHERE p.categoryId = @partitionKey")
                        .WithParameter("@partitionKey", "Garden");

// Query multiple items from container
using FeedIterator<Product> filteredFeed = container.GetItemQueryIterator<Product>(queryDefinition: parameterizedQuery);

// Iterate query result pages
while (filteredFeed.HasMoreResults)
{
    FeedResponse<Product> response = await filteredFeed.ReadNextAsync();

    // Iterate query results
    foreach (Product item in response)
    {
        await Console.Out.WriteLineAsync($"[Product {item.Id}]\tName: {item.Name,35}\tPrice:{item.Price,15:C}");
    }
}

#endregion

Console.WriteLine("Type the keyboard to view response from linq queryable...");
Console.ReadLine();

#region query_items_queryable
// Get LINQ IQueryable object
IOrderedQueryable<Product> queryable = container.GetItemLinqQueryable<Product>();

// Construct LINQ query
var matches = queryable
    .Where(p => p.CategoryId == "Garden")
    .Where(p => p.Price > 900.15d);

// Convert to feed iterator
using FeedIterator<Product> linqFeed = matches.ToFeedIterator();

// Iterate query result pages
while (linqFeed.HasMoreResults)
{
    FeedResponse<Product> response = await linqFeed.ReadNextAsync();

    // Iterate query results
    foreach (Product item in response)
    {
        await Console.Out.WriteLineAsync($"[Product {item.Id}]\tName: {item.Name,35}\tPrice:{item.Price,15:C}");
    }
}
#endregion

Console.WriteLine("Type the keyboard to view response indexMetrics...");
Console.ReadLine();

#region query_items_sql_parameters
var parameterizedQueryWithIndexMetrix =
                        new QueryDefinition(query: "SELECT * FROM products p WHERE p.categoryId = @partitionKey")
                        .WithParameter("@partitionKey", "Garden");

QueryRequestOptions options = new()
{
    PopulateIndexMetrics = true,
    MaxItemCount = 100
};

// Query multiple items from container
using FeedIterator<Product> iterator = container.GetItemQueryIterator<Product>(
    queryDefinition: parameterizedQueryWithIndexMetrix,
    requestOptions: options);

double totalRUs = 0;
// Iterate query result pages
while (iterator.HasMoreResults)
{
    FeedResponse<Product> response = await iterator.ReadNextAsync();
    Console.WriteLine($"{response.IndexMetrics}");

    // Iterate query results
    foreach (Product item in response)
    {
        await Console.Out.WriteLineAsync($"[Product {item.Id}]\tName: {item.Name,35}\tPrice:{item.Price,15:C}");
    }

    totalRUs += response.RequestCharge;
}

Console.WriteLine($"Total RUs:\t{totalRUs:0.00}");

#endregion

Console.WriteLine("Type the keyboard to exit...");
Console.ReadLine();