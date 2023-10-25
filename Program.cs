//To test this after upgrading to .NET 8, modify the .csproj file and set the TargetFramework to 8.0
//<TargetFramework>net8.0</TargetFramework>
//Then uncomment the attribute on line 22
using System.Text.Json;
using System.Text.Json.Serialization;

Console.WriteLine("Hello, World!");


CustomerInfo customer =
    JsonSerializer.Deserialize<CustomerInfo>("""{"Names":["John Doe"],"Company":{"Name":"Contoso"}}""")!;
// Output: {"Names":["John Doe"],"Company":{"Name":"Contoso","PhoneNumber":null}}
Console.WriteLine(customer.Company.Name);
Console.WriteLine(JsonSerializer.Serialize(customer));

class CompanyInfo
{
    public required string Name { get; set; }
    public string? PhoneNumber { get; set; }
}

[JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
class CustomerInfo
{
    // Both of these properties are read-only.
    public List<string> Names { get; } = new();
    public CompanyInfo Company { get; } = new() { Name = "N/A", PhoneNumber = "N/A" };
}
