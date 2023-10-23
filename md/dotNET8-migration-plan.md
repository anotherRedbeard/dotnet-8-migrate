# Migrate from .NET 5/6/7 to .NET 8.0

Here is a general guid on how to migrate from .NET core versions to 8

## Visual Studio

- Download [Visual Studio 2022 Preview](https://visualstudio.microsoft.com/vs/preview/#download-preview) with ASP.NET and web development workload.
- If you already have Visual Studio installed, you can go to the available tab in the installer and you should see it there to update.

## Visual Studio Code

- Download [Visual Studio Code](https://code.visualstudio.com/download)
- Install the [C# for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) extension
- Install the [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

VS Code uses the .NET CLI for things like project creation. You can use this on macOS, Linux, or Windows with any code editor.

### Update the .NET SDK version in global.json

If you rely on a `global.json` file to target a specific .NET Core SDK version, update the version property to the .NET 8.0 SDK version that's installed. For example:

```diff
{
  "sdk": {
-    "version": "6.0.200"
+    "version": "8.0.100"
  }
}
```

### Update the target framework

Update the .csproj file's Target Framework moniker to `net8.0`

```diff
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
-    <TargetFramework>net7.0</TargetFramework>
+    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

</Project>
```

### Update package references

You will want to look inside the .csproj for any references that need to be updated to the new .NET 8. The [Microsoft.AspNetCore.*](https://www.nuget.org/packages?q=Microsoft.AspNetCore.*), [Microsoft.EntityFrameworkCore.*](https://www.nuget.org/packages?q=Microsoft.EntityFrameworkCore.*), [Microsoft.Extensions.*](https://www.nuget.org/packages?q=Microsoft.Extensions.*), and [System.Net.Http.Json]() packages is what you would want to look for and set the version attribute to 8.0.0.

```diff
<ItemGroup>
-   <PackageReference Include="Microsoft.AspNetCore.JsonPatch" Version="7.0.12" />
-   <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="7.0.12" />
-   <PackageReference Include="Microsoft.Extensions.Caching.Abstractions" Version="7.0.0" />
-   <PackageReference Include="System.Net.Http.Json" Version="7.0.1" />
+   <PackageReference Include="Microsoft.AspNetCore.JsonPatch" Version="8.0.0" />
+   <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0">
+   <PackageReference Include="Microsoft.Extensions.Caching.Abstractions" Version="8.0.0" />
+   <PackageReference Include="System.Net.Http.Json" Version="8.0.0" />
</ItemGroup>
```

You can also update these versions by running the package restore command.  Something like this:  `dotnet add package Microsoft.AspNetCore.JsonPatch --version 8.0.0-rc.2.23479.6`

## More Info

You can [Learn More](https://learn.microsoft.com/en-us/aspnet/core/migration/70-80?view=aspnetcore-7.0&tabs=visual-studio) here by checking out the .NET documentation