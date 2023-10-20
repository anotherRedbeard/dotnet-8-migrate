# Highlight Breaking Changes

1. **Identifying Breaking Changes**
    - Most changes are from planned deprecations or infra support changes, but MFST provides a document that outlines these changes here:
        - [.Net 8](https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0?toc=%2Fdotnet%2Ffundamentals%2Ftoc.json&bc=%2Fdotnet%2Fbreadcrumb%2Ftoc.json)
        - [.Net 7](https://learn.microsoft.com/en-us/dotnet/core/compatibility/7.0?toc=%2Fdotnet%2Ffundamentals%2Ftoc.json&bc=%2Fdotnet%2Fbreadcrumb%2Ftoc.json)
    - You can also to to the [Issues of .NET](https://issuesof.net/?q=%20is:open%20-label:Documented%20is:issue%20(label:%22Breaking%20Change%22%20or%20label:breaking-change)%20(repo:dotnet/docs%20or%20repo:aspnet/Announcements)%20group:repo%20(label:%22:checkered_flag:%20Release:%20.NET%208%22%20or%20label:8.0.0)%20sort:created-desc) to see anything that might not have made it to the documents yet.
2. **Categorizing Change**
    - Each change is categorized as one of the following:
        - **Binary incompatible** - Breaking change when running code against the new runtime, recompile with new runtime
        - **Source incompatible** - Breaking change at compile time, source change might be required
        - **Behavioral change** - Existing code may behave differently at runtime, if not expected, code change would be required
    - Given these changes, we will focus more on the **Source incompatible** types as those typically indicate a code change.
3. **.NET Categories**
    | Topic                | Source incompatible | Binary incompatible | Behavioral change  |
    |----------------------|-------------------|----------------------|-----------------------|
    | [ASP.NET Core](https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0?toc=%2Fdotnet%2Ffundamentals%2Ftoc.json&bc=%2Fdotnet%2Fbreadcrumb%2Ftoc.json#aspnet-core)         | 3                 | 0                    | 3                     |
    | [Containers](https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0?toc=%2Fdotnet%2Ffundamentals%2Ftoc.json&bc=%2Fdotnet%2Fbreadcrumb%2Ftoc.json#containers)           | 0                 | 2                    | 5                     |
    | [Core .NET Libraries](https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0?toc=%2Fdotnet%2Ffundamentals%2Ftoc.json&bc=%2Fdotnet%2Fbreadcrumb%2Ftoc.json#core-net-libraries)  | 4                 | 2                    | 12                     |
    | [Cryptography](https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0?toc=%2Fdotnet%2Ffundamentals%2Ftoc.json&bc=%2Fdotnet%2Fbreadcrumb%2Ftoc.json#cryptography)         | 1                 | 0                    | 1                     |
    | [Deployment](https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0?toc=%2Fdotnet%2Ffundamentals%2Ftoc.json&bc=%2Fdotnet%2Fbreadcrumb%2Ftoc.json#deployment)           | 0                 | 1                    | 2                     |
    | [Extensions](https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0?toc=%2Fdotnet%2Ffundamentals%2Ftoc.json&bc=%2Fdotnet%2Fbreadcrumb%2Ftoc.json#extensions)           | 2                 | 0                    | 6                     |
    | [Globalization](https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0?toc=%2Fdotnet%2Ffundamentals%2Ftoc.json&bc=%2Fdotnet%2Fbreadcrumb%2Ftoc.json#globalization)        | 0                 | 0                    | 2                     |
    | [Interop](https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0?toc=%2Fdotnet%2Ffundamentals%2Ftoc.json&bc=%2Fdotnet%2Fbreadcrumb%2Ftoc.json#interop)              | 2                 | 1                    | 1                     |
    | [Reflection](https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0?toc=%2Fdotnet%2Ffundamentals%2Ftoc.json&bc=%2Fdotnet%2Fbreadcrumb%2Ftoc.json#reflection)           | 0                 | 0                    | 1                     |
    | [SDK](https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0?toc=%2Fdotnet%2Ffundamentals%2Ftoc.json&bc=%2Fdotnet%2Fbreadcrumb%2Ftoc.json#sdk)                  | 5                 | 3                    | 11                     |
    | [Serialization](https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0?toc=%2Fdotnet%2Ffundamentals%2Ftoc.json&bc=%2Fdotnet%2Fbreadcrumb%2Ftoc.json#serialization)        | 0                 | 0                    | 2                     |
    | [Windows Forms](https://learn.microsoft.com/en-us/dotnet/core/compatibility/8.0?toc=%2Fdotnet%2Ffundamentals%2Ftoc.json&bc=%2Fdotnet%2Fbreadcrumb%2Ftoc.json#windows-forms)        | 1                 | 0                    | 7                     |

    - All these breaking changes can be found by digging into each category and going to the corresponding link on the change.  This will list the Version introduced, Previous behavior, New behavior, Type of breaking change, Reason for change, and Recommended action
    - Entity Framework Core if a little bit different so is a chart of the degree of changes
        |High|Medium|Low|
        |----|------|---|
        |2|2|5|

        - High
            - Contains in LINQ queries is moving to a new more efficient query rendering and is unsupported at SQL Server 2014 and below. This may also impact newer SQL Server versions with an older compatibility level
                - If you can't mitigate this by upgrading to using a compatibility level above 130 you can adjust the compatibility level in EF Core

                    ```csharp
                    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                        => optionsBuilder
                            .UseSqlServer(@"<CONNECTION STRING>", o => o.UseCompatibilityLevel(120));
                    ```

            - Enums in JSON are stored as ints instead of strings by default.
                - If you can't adjust your code, you can mitigate this in one of two ways.
                - Use a conversion

                    ```csharp
                    protected override void OnModelCreating(ModelBuilder modelBuilder)
                        {
                            modelBuilder.Entity<User>().Property(e => e.Status).HasConversion<string>();
                        }
                    ```

                - Or for all properties of the enum type

                    ```csharp
                    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
                        {
                            configurationBuilder.Properties<StatusEnum>().HaveConversion<string>();
                        }
                    ```

        - To see the medium and low impact changes, please visit [this document](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-8.0/breaking-changes)
