// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using System.Text.Json.Serialization;

Console.WriteLine("Hello, World!");


CustomerInfo customer =
    JsonSerializer.Deserialize<CustomerInfo>("""{"Names":["John Doe"],"Company":{"Name":"Contoso"}}""")!;
// Output: {"Names":["John Doe"],"Company":{"Name":"Contoso","PhoneNumber":null}}
Console.WriteLine(JsonSerializer.Serialize(customer));

class CompanyInfo
{
    public required string Name { get; set; }
    public string? PhoneNumber { get; set; }
}

//[JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
class CustomerInfo
{
    // Both of these properties are read-only.
    public List<string> Names { get; } = new();
    public CompanyInfo Company { get; } = new() { Name = "N/A", PhoneNumber = "N/A" };
}
