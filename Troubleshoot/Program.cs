using Microsoft.Azure.Cosmos;
using Shareds;
using System.Text.Json;
using Troubleshoot;

var cosmosClientOptions = new CosmosClientOptions()
{
    AllowBulkExecution = true,
    MaxRetryAttemptsOnRateLimitedRequests = 50,
    MaxRetryWaitTimeOnRateLimitedRequests = new TimeSpan(0, 1, 30)
};

CosmosClient client = CosmosClientSingleton.Create(cosmosClientOptions);
Console.WriteLine("-- CosmosClientOptions JSON --");
Console.WriteLine($"{JsonSerializer.Serialize(client.ClientOptions, DefaultJsonSerializerOptions.Instance)}");

Database database = await client.CreateDatabaseIfNotExistsAsync("cosmicworks");
Container container = await database.CreateContainerIfNotExistsAsync(id: "products", partitionKeyPath: "/categoryId", throughput: 400);

int endWhile = 99;
Simulators.ConsoleMenu(endWhile.ToString());

string consoleinputcharacter;

while ((consoleinputcharacter = Console.ReadLine()!) != endWhile.ToString())
{
    try
    {
        await Simulators.CompleteTaskOnCosmosDB(consoleinputcharacter, container);
    }
    catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.Conflict)
    {
        Console.WriteLine("Insert Failed. Response Code (409).");
        Console.WriteLine("Can not insert a duplicate partition key, customer with the same ID already exists.");
    }
    catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.Forbidden)
    {
        Console.WriteLine("Response Code (403).");
        Console.WriteLine("The request was forbidden to complete. Some possible reasons for this exception are:");
        Console.WriteLine("Firewall blocking requests.");
        Console.WriteLine("Partition key exceeding storage.");
        Console.WriteLine("Non-data operations are not allowed.");
    }
    catch (CosmosException e) when (e.StatusCode is System.Net.HttpStatusCode.TooManyRequests
                                        or System.Net.HttpStatusCode.ServiceUnavailable
                                        or System.Net.HttpStatusCode.RequestTimeout)
    {
        // Check if the issues are related to connectivity and if so, wait 10 seconds to retry.
        await Task.Delay(10_000); // Wait 10 seconds
        try
        {
            Console.WriteLine("Try one more time...");
            await Simulators.CompleteTaskOnCosmosDB(consoleinputcharacter, container);
        }
        catch (CosmosException e2)
        {
            Console.WriteLine("Insert Failed. " + e2.Message);
            Console.WriteLine("Can not insert a duplicate partition key, Connectivity issues encountered.");
            break;
        }
    }
    catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
        Console.WriteLine("Delete Failed. Response Code (404).");
        Console.WriteLine("Can not delete customer, customer not found.");
    }
    catch (CosmosException e)
    {
        Console.WriteLine(e.Message);
    }

    Simulators.ConsoleMenu(endWhile.ToString());
}