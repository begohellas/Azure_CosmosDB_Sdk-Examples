using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Shareds;
public static class CosmosClientSingleton
{
    private static readonly IConfiguration Configuration = new ConfigurationBuilder()
           .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
           .AddJsonFile("appsettings.json", false, true)
           .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
           .Build();

    static CosmosClientSingleton()
    { }

    public static CosmosClient Create(CosmosClientOptions cosmosClientOptions = null)
    {
        return new CosmosClient(Configuration["Endpoint"], Configuration["MasterKey"], cosmosClientOptions);
    }
}
