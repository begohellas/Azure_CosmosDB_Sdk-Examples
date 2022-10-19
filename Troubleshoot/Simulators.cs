using Microsoft.Azure.Cosmos;
using Shareds.Models;

namespace Troubleshoot;
public static class Simulators
{
    public static async Task CompleteTaskOnCosmosDB(string consoleinputcharacter, Container container)
    {
        switch (consoleinputcharacter)
        {
            case "1":
                await CreateDocument1(container);
                break;
            case "2":
                await CreateDocument2(container);
                break;
            case "3":
                await DeleteDocument1(container);
                break;
            case "4":
                await DeleteDocument2(container);
                break;
            default:
                Console.WriteLine("Choiche wrong, repeat insert");
                break;
        }
        Console.Clear();
    }
    private static async Task CreateDocument1(Container containerProduct)
    {
        const string product1 = "0C297972-BE1B-4A34-8AE1-F39E6AA3D828";
        const string category = "Toys";

        Product product = new()
        {
            Id = product1,
            CategoryId = category,
            Name = "Lego Batman",
            Price = 35.99d,
            Tags = new string[]
            {
                 "new",
                 "eccomerce"
            }
        };

        Console.Clear();

        ItemResponse<Product> response = await containerProduct.CreateItemAsync<Product>(product, new PartitionKey(category));
        Console.WriteLine("Insert Successful.");
        Console.WriteLine($"Document for product with id = '{product1}' Inserted.");

        Console.WriteLine("Press [ENTER] to continue");
        Console.ReadLine();

    }
    private static async Task CreateDocument2(Container containerProduct)
    {
        const string product2 = "AAFF2225-A5DD-4318-A6EC-B056F96B94B7";
        const string category = "Toys";

        Product product = new()
        {
            Id = product2,
            CategoryId = category,
            Name = "Lego Jurassik Park",
            Price = 48.99d,
            Tags = new string[]
            {
                 "new",
                 "eccomerce"
            }
        };

        Console.Clear();

        ItemResponse<Product> response = await containerProduct.CreateItemAsync<Product>(product, new PartitionKey(category));
        Console.WriteLine("Insert Successful.");
        Console.WriteLine($"Document for customer with id = '{product2}' Inserted.");

        Console.WriteLine("Press [ENTER] to continue");
        Console.ReadLine();
    }

    private static async Task DeleteDocument1(Container Customer)
    {
        const string product1 = "0C297972-BE1B-4A34-8AE1-F39E6AA3D828";
        const string category = "Toys";

        Console.Clear();

        ItemResponse<Product> response = await Customer.DeleteItemAsync<Product>(partitionKey: new PartitionKey(category), id: product1);
        Console.WriteLine("Delete Successful.");
        Console.WriteLine($"Document for customer with id = '{product1}' Deleted.");


        Console.WriteLine("Press [ENTER] to continue");
        Console.ReadLine();
    }
    private static async Task DeleteDocument2(Container Customer)
    {
        const string product2 = "AAFF2225-A5DD-4318-A6EC-B056F96B94B7";
        const string category = "Toys";

        Console.Clear();

        ItemResponse<Product> response = await Customer.DeleteItemAsync<Product>(partitionKey: new PartitionKey(category), id: product2);
        Console.WriteLine("Delete Successful.");
        Console.WriteLine($"Document for customer with id = '{product2}' Deleted.");


        Console.WriteLine("Press [ENTER] to continue");
        Console.ReadLine();
    }

    public static void ConsoleMenu(string endWhile)
    {
        Console.WriteLine("#############################################");
        Console.WriteLine("1) Add Document 1 with id = '0C297972-BE1B-4A34-8AE1-F39E6AA3D828'");
        Console.WriteLine("2) Add Document 2 with id = 'AAFF2225-A5DD-4318-A6EC-B056F96B94B7'");
        Console.WriteLine("3) Delete Document 1 with id = '0C297972-BE1B-4A34-8AE1-F39E6AA3D828'");
        Console.WriteLine("4) Delete Document 2 with id = 'AAFF2225-A5DD-4318-A6EC-B056F96B94B7'");
        Console.WriteLine($"{endWhile}) Exit");
        Console.Write("\r\nSelect an option: ");
    }
}
