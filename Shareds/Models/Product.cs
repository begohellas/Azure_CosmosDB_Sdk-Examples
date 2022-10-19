using Newtonsoft.Json;

namespace Shareds.Models;
public class Product
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("categoryId")]
    public string CategoryId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("price")]
    public double Price { get; set; }

    [JsonProperty("tags")]
    public string[] Tags { get; set; }
}