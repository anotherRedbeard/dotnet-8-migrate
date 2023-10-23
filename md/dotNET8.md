# Highlight New C# Features in .NET 8

1. **.NET Core Libraries**

   1. **Serialization, JSON Enhancements**
      - Custom handling of missing members during deserialization.
      - Built-in support for additional types:
         - More supported types: Half, Int128, Uint 128, Memory<T>, and ReadOnly Memory<T>.
      - Source Generator:
         - By default, System.Text.Json uses run-time reflection to gather metadata for serialization and deserialization. Source Generation, usable with .NET 6 and later, improves performance. It can automatically generate serialization code for classes, making object-to-JSON conversion more efficient and customizable. It allows developers to specify JSON serialization rules for their types, including custom naming policies and other serialization options.
         - New updates to the source generator.
         - [Learn More](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation?pivots=dotnet-8-0)
      - Interface Hierarchies:
         - Support for serializing properties from interface hierarchies.

         	```csharp
            IDerived value = new DerivedImplement { Base = 0, Derived = 1 };
            JsonSerializer.Serialize(value); // {"Base":0,"Derived":1}
            
            public interface IBase
            {
                  public int Base { get; set; }
            }
            
            public interface IDerived : IBase
            {
                  public int Derived { get; set; }
            }
            
            public class DerivedImplement : IDerived
            {
                  public int Base { get; set; }
                  public int Derived { get; set; }
            }
            ```

      - Naming Policies:
         - Introduction of naming policies like snake_case and kebab-case for property name conversions.
         
         ```csharp
         var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
         JsonSerializer.Serialize(new { PropertyName = "value" }, options); // { "property_name" : "value" }
         ```

      - Read-only Properties:
         - Deserialization support for read-only fields or properties. Can be opted into globally or granularly using attributes.

         ```csharp
         using System.Text.Json;
         CustomerInfo customer =
             JsonSerializer.Deserialize<CustomerInfo>("""{"Names":["John Doe"],"Company":{"Name":"Contoso"}}""")!;
         // Output: {"Names":["John Doe"],"Company":{"Name":"Contoso","PhoneNumber":null}}
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
         ```
      - Disable Reflection-based Default:
         - Option to disable reflection-based serialization by default.
      - New JsonNode API Methods:
         - Additional methods for working with JSON nodes and arrays.

         ```csharp
            public partial class JsonNode
            {
                  // Creates a deep clone of the current node and all its descendants.
                  public JsonNode DeepClone();
            
                  // Returns true if the two nodes are equivalent JSON representations.
                  public static bool DeepEquals(JsonNode? node1, JsonNode? node2);
            
                  // Determines the JsonValueKind of the current node.
                  public JsonValueKind GetValueKind(JsonSerializerOptions options = null);
            
                  // If node is the value of a property in the parent
                  // object, returns its name.
                  // Throws InvalidOperationException otherwise.
                  public string GetPropertyName();
            
                  // If node is the element of a parent JsonArray,
                  // returns its index.
                  // Throws InvalidOperationException otherwise.
                  public int GetElementIndex();
            
                  // Replaces this instance with a new value,
                  // updating the parent object/array accordingly.
                  public void ReplaceWith<T>(T value);
            
                  // Asynchronously parses a stream as UTF-8 encoded data
                  // representing a single JSON value into a JsonNode.
                  public static Task<JsonNode?> ParseAsync(
                     Stream utf8Json,
                     JsonNodeOptions? nodeOptions = null,
                     JsonDocumentOptions documentOptions = default,
                     CancellationToken cancellationToken = default);
            }
            
            public partial class JsonArray
            {
                  // Returns an IEnumerable<T> view of the current array.
                  public IEnumerable<T> GetValues<T>();
            }
         ```

      - Non-Public Members:
         - Option to include non-public members in the serialization contract.
         ```csharp
         string json = JsonSerializer.Serialize(new MyPoco(42)); // {"X":42}
         JsonSerializer.Deserialize<MyPoco>(json);
         
         public class MyPoco
         {
             [JsonConstructor]
             internal MyPoco(int x) => X = x;
         
             [JsonInclude]
             internal int X { get; }
         }
         ```
      - Streaming Deserialization APIs:
         - Introduction of IAsyncEnumerable<T> streaming deserialization extension methods.
         ```csharp
         const string RequestUri = "https://api.contoso.com/books";
         using var client = new HttpClient();
         IAsyncEnumerable<Book> books = client.GetFromJsonAsAsyncEnumerable<Book>(RequestUri);
         
         await foreach (Book book in books)
         {
             Console.WriteLine($"Read book '{book.title}'");
         }
         ```
      - WithAddedModifier Extension Method:
         - Extending serialization contracts with modifiers.
         ```csharp
         var options = new JsonSerializerOptions
         {
             TypeInfoResolver = MyContext.Default
                 .WithAddedModifier(static typeInfo =>
                 {
                     foreach (JsonPropertyInfo prop in typeInfo.Properties)
                         prop.Name = prop.Name.ToUpperInvariant();
                 })
         };
         ```

      - New JsonContent.Create Overloads:
         - Methods to create JsonContent instances with trim-safe or source-generated contracts.

         ```csharp
         var book = new Book(id: 42, "Title", "Author", publishedYear: 2023);
         HttpContent content = JsonContent.Create(book, MyContext.Default.Book);
         ```

      - Freeze a JsonSerializerOptions Instance:
         - Methods for controlling the freezing of a JsonSerializerOptions instance after it has been set up.
         - There is also an IsReadOnly property that you can check on an existing JsonSerializerOptions object.

   2. **Time Abstraction**
      - Allows you to create a time provider that works with a time zone that is different than local time.

         ```csharp
         // Get system time.
         DateTimeOffset utcNow = TimeProvider.System.GetUtcNow();
         DateTimeOffset localNow = TimeProvider.System.GetLocalNow();
         
         // Create a time provider that works with a
         // time zone that's different than the local time zone.
         private class ZonedTimeProvider : TimeProvider
         {
            private TimeZoneInfo _zoneInfo;
         
            public ZonedTimeProvider(TimeZoneInfo zoneInfo) : base()
            {
                  _zoneInfo = zoneInfo ?? TimeZoneInfo.Local;
            }
         
            public override TimeZoneInfo LocalTimeZone => _zoneInfo;
         
            public static TimeProvider FromLocalTimeZone(TimeZoneInfo zoneInfo) =>
                  new ZonedTimeProvider(zoneInfo);
         }
         
         // Create a timer using a time provider.
         ITimer timer = timeProvider.CreateTimer(callBack, state, delay, Timeout.InfiniteTimeSpan);
         
         // Measure a period using the system time provider.
         long providerTimestamp1 = TimeProvider.System.GetTimestamp();
         long providerTimestamp2 = TimeProvider.System.GetTimestamp();
         
         var period = GetElapsedTime(providerTimestamp1, providerTimestamp2);
         ```

      - [Learn More](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#time-abstraction)

   3. **Randomness Tools**
      - **GetItems()**
         - Let you randomly choose a specified number of items from an input set.

            ```csharp
            private static ReadOnlySpan<Button> s_allButtons = new[]
            {
                  Button.Red,
                  Button.Green,
                  Button.Blue,
                  Button.Yellow,
            };
            
            // ...
            
            Button[] thisRound = Random.Shared.GetItems(s_allButtons, 31);
            // Rest of the game goes here …
            ```

         - [Learn More](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#getitemst)

      - **Shuffle()**
         - Let you randomize the order of a span.

            ```csharp
            YourType[] trainingData = LoadTrainingData();
            Random.Shared.Shuffle(trainingData);
            
            IDataView sourceData = mlContext.Data.LoadFromEnumerable(trainingData);
            
            DataOperationsCatalog.TrainTestData split = mlContext.Data.TrainTestSplit(sourceData);
            model = chain.Fit(split.TrainSet);
            
            IDataView predictions = model.Transform(split.TestSet);
            // …
            ```

         - [Learn More](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#shufflet)
   4. **Performance-focused Types**
      - **System.Collections.Frozen**
        - Doesn't allow changes once the collection is created.

        ```csharp
        private static readonly FrozenDictionary<string, bool> s_configurationData =
            LoadConfigurationData().ToFrozenDictionary(optimizeForReads: true);
        
        // ...
        if (s_configurationData.TryGetValue(key, out bool setting) && setting)
        {
            Process();
        }
        ```

      - **System.Buffers.SearchValues<T>**
        - A read-only set of values optimized for efficient searching, used for methods that look for the first occurrence of any value in the passed collection.
      - **System.TextCompositeFormat**
        - Optimizes format strings that aren't known at compile time.

        ```csharp
        private static readonly CompositeFormat s_rangeMessage =
            CompositeFormat.Parse(LoadRangeMessageResource());
        
        // ...
        static string GetMessage(int min, int max) =>
            string.Format(CultureInfo.InvariantCulture, s_rangeMessage, min, max);
        ```

      - **System.IO.Hashing.XxHash3, System.IO.Hashing.XxHash128**
        - Implementations of the fast XXH# and XXH128 hash algorithms.

        ```csharp
        private static readonly FrozenDictionary<string, bool> s_configurationData =
            LoadConfigurationData().ToFrozenDictionary(optimizeForReads: true);
        
        // ...
        if (s_configurationData.TryGetValue(key, out bool setting) && setting)
        {
            Process();
        }
        ```

   5. **Data Validation**
      - New data validation attributes intended for more cloud-native services and are designed to validate non-user-entry data link configuration options
      - [Learn More](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#data-validation)

   6. **Cryptography Enhancements**
      - SHA-3 support for hashing primitives

         ```csharp
         // Hashing example
         if (SHA3_256.IsSupported)
         {
            byte[] hash = SHA3_256.HashData(dataToHash);
         }
         else
         {
            // ...
         }
         
         // Signing example
         if (SHA3_256.IsSupported)
         {
               using ECDsa ec = ECDsa.Create(ECCurve.NamedCurves.nistP256);
               byte[] signature = ec.SignData(dataToBeSigned, HashAlgorithmName.SHA3_256);
         }
         else
         {
            // ...
         }
         ```

      - [Learn More](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#cryptography)
   7. **Networking**
      - Support for HTTPS proxy
         - Until now the proxy types that HttpClient supported allowed a 'man-in-the-middle' to see the connecting client, now HttpClient supports HTTPS proxy to create an encrypted channel
         - User the WebProxy class to control the proxy programatically
   8. **Stream-based ZipFile methods**
      - New overloads to support ziping and streaming the zip file
      - Useful when disk space is constrained because they avoid having to use the disk as an intermediate step

2. **Extension Libraries**
   1. **Keyed Dependency Injection (DI) Services**
     - Provides a way for registering and retrieving DI services using keys.

     ```csharp
     using Microsoft.Extensions.Caching.Memory;
     using Microsoft.Extensions.Options;

     var builder = WebApplication.CreateBuilder(args);

     builder.Services.AddSingleton<BigCacheConsumer>();
     builder.Services.AddSingleton<SmallCacheConsumer>();

     builder.Services.AddKeyedSingleton<IMemoryCache, BigCache>("big");
     builder.Services.AddKeyedSingleton<IMemoryCache, SmallCache>("small");

     var app = builder.Build();

     app.MapGet("/big", (BigCacheConsumer data) => data.GetData());
     app.MapGet("/small", (SmallCacheConsumer data) => data.GetData());

     app.Run();

     class BigCacheConsumer([FromKeyedServices("big")] IMemoryCache cache)
     {
         public object? GetData() => cache.Get("data");
     }

     class SmallCacheConsumer(IKeyedServiceProvider keyedServiceProvider)
     {
         public object? GetData() => keyedServiceProvider.GetRequiredKeyedService<IMemoryCache>("small");
     }
     ```

     - [Learn More](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#keyed-di-services)

   2. **Hosted Lifecycle Services**
     - Lifecycle methods include `StartingAsync(CancellationToken)`, `StartedAsync(CancellationToken)`, `StoppingAsync(CancellationToken)`, and `StoppedAsync(CancellationToken)`.
     - [Learn More](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#hosted-lifecycle-services)

   3. **Options Validation**
     - **Source Generator**
       - The source code generator in .NET 8 simplifies options validation. It generates validation logic for your configuration models.
       - In C#, you can define models with attributes like `[Required]` and `[MinLength(5)]` to enforce constraints on configuration properties.
       - The generated code includes validator classes implementing `IValidateOptions<T>`, which you can inject into your app for dependency injection.
     
     - **LoggerMessageAttribute Constructors**
       - LoggerMessageAttribute now offers more constructor overloads, improving how you define log messages.
       - You can specify the log level and message separately, allowing greater flexibility.
       - If you don't provide an event ID, the system automatically generates one.

     - **Extensions Metrics**
       - **IMeterFactory Interface**: This interface allows you to create `Meter` objects in a more isolated manner. You can use it to register metrics in dependency injection containers.
       - **MetricCollector<T> Class**: This class enables you to record metric measurements along with timestamps. You can choose a time provider for precise timestamp generation.
       - You can create counters and other metric instruments and record their values along with timestamps.

     - **System.Numerics.Tensors.TensorPrimitives**
       - The `System.Numerics.Tensors` package has been updated to include a new namespace, `TensorPrimitives`.
       - This update is particularly useful for data-intensive workloads such as AI and machine learning.
       - It provides support for tensor operations, making it easier to perform operations on vectors, which are essential for AI workloads.
       - This package optimizes tasks like semantic search and retrieval-augmented generation (RAG) for AI models like ChatGPT by streamlining vector operations like cosine similarity.

     - [Learn More](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#options-validation)

3. **Garbage Collector Improvements**
   - Ability to adjust memory limits on-the-fly, ideal for cloud services that experience fluctuating demand.
   - Scale resource consumption up or down in response to changing demand, ensuring cost-effective resource utilization.
   - You can use the RefreshMemoryLimit() API to update memory limits
   - [Learn More](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#garbage-collection)

4. **Native (Ahead of Time) AOT support**
   - First introduced in .NET 7
   - Creates a fully self-contained version of your app that doesn't need a runtime
   - Improvements in .NET 8
      - Supports x64 and Arm64 on macOS
      - Reduces the size of Native AOT apps on Linux by 50%
      - Allows you to specify size or speed optimizations when compiling
      - Default template includes AOT support
         - Run `dotnet new console --aot` to create an AOT-enabled project
      - Target iOS-like platforms
         - Early testing shows app size on disk decreases by ~40% over Mono
      - Mono is still the default for app development, but you can set the following properties to use AOT in the .csproj file
         - PublishAot
         - PublishAotUsingRuntimePack

            ```csharp
            <PropertyGroup>
               <PublishAot>true</PublishAot>
               <PublishAotUsingRuntimePack>true</PublishAotUsingRuntimePack>
            </PropertyGroup>
            ```

5. **Performance improvements**
   - .NET 8 introduces significant enhancements for improved execution speed and code efficiency, focusing on:
      - **Arm64 Support**: Enhanced performance for Arm64 architecture.
      - **SIMD Processing**: Improved Single Instruction, Multiple Data (SIMD) capabilities.
      - **Cloud-Native Development**: Enhancements tailored for cloud-native scenarios.
      - **Optimization Techniques**: Streamlined optimization strategies.

   - For more in-depth insights and detailed results, you can explore this [blog](https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-8/).

6. **.NET SDK Updates**
   - **CLI-based project evaluation**
      - Incorporate data from MSBuild into CI pipelines or elsewhere
   - **Terminal build output**
      - Terminal logger `--tl` option groups errors with the project they cam from
   - **Simplified output paths**
      - Use `ArtifactsPath` property or set the `UseArtifactsOutput` to true to use the default

         ```csharp
            📁 artifacts
            └──📂 <Type of output>
               └──📂 <Project name>
                     └──📂 <Pivot>

   - **`dotnet workload clean` command**
      - Clean up workload packs that might be left over through several updates
   - **Publish pack features are now automatically set to release.**
      - To revert back to previous versions you can use `dotnet publish -p:PublishRelease=false` command
   - **`dotnet restore` security auditing**
      - Opt into security checks for known vulnerabilities when dependency packages are restored
   - **Template engine**
      - More secure experience by integrating some of NuGet's security-related features
      - To proceed when it fails you will need to use the `--force` flag
   - **Source Link**
      - Source Link is now included in hopes that more packages will include this information by default.
   - [Learn More](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#net-sdk)

7. **Globalization**
   - Mobile apps can use a new hybrid globalization mode that uses a lighter ICU bundle.
   - Most suitable for apps that can't work in `InvariantGlobalization` mode and that use cultures that were trimmed from ICU data on mobile.
   - [Learn More](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#globalization)

8. **Containers**
   - Containers solve the 'Works on my Machine' problem, providing benefits such as agility, portability, and density.
      - **Container Images**
         - Default container tag is now `latest`
         - Use Debian 12 by default
         - Non-root user by adding `USER app` to the end of Dockerfile
         - Default port changed to 8080 and is available in the `ASPNETCORE_HTTP_PORTS` env variable
         - Build multi-platform container images
         - Performance improvements on pushing containers to remote registries and more are supported
         - Authentication support for Azure Managed Identity when pushing containers to registries
         - You can containerize a .NET app with `dotnet publish`. You will need the Docker daemon
            - [Learn More](https://learn.microsoft.com/en-us/dotnet/core/docker/publish-as-container?pivots=dotnet-7-0)
         - Changes have been made to the Alpine images to upgrade them to newer versions of Linux distributions.
            - [Learn More](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#containers)
         - Create a container directly as a tar.gz archive. Now you can publish the container as an artifact

9. **.NET on Linux**
   - Now targeting Ubuntu 16.04 for all architectures
   - You can now build .NET on Linux directly from the dotnet/dotnet repo
   - [Learn More](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#net-on-linux)

1. **Cross-built Windows apps**
   - Building apps that target Windows on a non-Windows platform will result in an executable with any specified Win32 resources--for example, application icon, manifest, version information
   - [Learn More](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#cross-built-windows-apps)

1. **Code Analysis**
   - New code analyzers and fixers have been added to help verify you are using the .NET library APIs correctly and efficiently
   - [Learn More](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#code-analysis)

1. **NuGet**
   - NuGet now verifies signed packages on Linux by default
   - [Learn More](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#nuget)