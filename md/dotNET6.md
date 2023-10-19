##Nullable Reference Types (NRT) & Top-Level Statements

- **NRT (Nullable Reference Types)**
  - While nothing has really changed per se here, it's essential to understand that these are significant changes from .NET 6. Let's review them at a high level:
    - **Null State Analysis**
      - Variables are either not-null or maybe-null, which determines if they can be dereferenced or not.
      - Example:
        ```csharp
        string message = null;
        
        // warning: dereference null. (maybe-null)
        Console.WriteLine($"The length of the message is {message.Length}");
        
        var originalMessage = message;
        message = "Hello, World!";
        
        // No warning. Analysis determined "message" is not null. (not-null)
        Console.WriteLine($"The length of the message is {message.Length}");
        
        // warning! (maybe-null)
        Console.WriteLine(originalMessage.Length);
        ```
      - By default, null state analysis doesn't trace into methods, so it will always generate a warning unless you use 'Constructor Chaining' or 'Nullable Attributes' on the method.
      - Example with Nullable Attributes:
        ```csharp
        using System.Diagnostics.CodeAnalysis;
        
        public class Person
        {
            public string FirstName { get; set; }
            public string LastName { get. set; }
            
            public Person(string firstName, string lastName)
            {
                FirstName = firstName;
                LastName = lastName;
            }
            
            public Person() : this("John", "Doe") { }
        }
        
        public class Student : Person
        {
            public string Major { get; set; }
            
            public Student(string firstName, string lastName, string major)
                : base(firstName, lastName)
            {
                SetMajor(major);
            }
            
            public Student(string firstName, string lastName) :
                base(firstName, lastName)
            {
                SetMajor();
            }
            
            public Student()
            {
                SetMajor();
            }
            
            [MemberNotNull(nameof(Major))]
            private void SetMajor(string? major = default)
            {
                Major = major ?? "Undeclared";
            }
        }
        ```
    - **Attributes on API Signatures**
      - Null state analysis needs hints from developers to understand the semantics of the APIs. Developers need to add certain attributes on the signatures to give the compiler hints. For example:
        ```csharp
        public static bool IsNullOrWhiteSpace([NotNullWhen(false)] string message);
        ```
    - **Nullable Variable Annotations**
      - The compiler needs more help from developers for member variables to determine if a reference can be null or may be null. This is done using nullable reference types. For example, you would declare `string? Name;` to tell the compiler that `Name` could be null.
      - To override the warning when you know a variable isn't null, you can use the null-forgiving operator `!` to force the null-state to be not-null. For example: `Name!.Length`
  - For more information, visit [here](https://learn.microsoft.com/en-us/dotnet/csharp/nullable-references).

- **Top-Level Statements**
  - This refers to how we can now remove the ceremony around the entry point into an app. We go from this:
    ```csharp
    using System;
    
    namespace Application
    {
        class Program
        {
            static void Main(string[] args)
            {
                Console.WriteLine("Hello World!");
            }
        }
    }
    ```
    To this:
    ```csharp
    // See [here](https://aka.ms/new-console-template) for more information
    Console.WriteLine("Hello, World!");
    ```
  - Key Points:
    - Streamlined entry point to a program
    - Ideal for scripting and small utility programs
    - No need for containing class
    - Compiler automatically generates the default 'Main' method
  - For more details, visit [here](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/top-level-statements).
