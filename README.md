# Olibos.Data.SqlClient.SqlBulkCopy

[![NuGet](https://img.shields.io/nuget/v/Olibos.Data.SqlClient.SqlBulkCopy)](https://www.nuget.org/packages/Olibos.Data.SqlClient.SqlBulkCopy/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

A high-performance C# source generator that automatically creates SQL bulk copy adapters for your domain models. Eliminates boilerplate code and enables seamless bulk data insertion into SQL Server using `IAsyncEnumerable<T>`.

## Features

✨ **Zero Boilerplate** - Automatic code generation for bulk copy operations  
⚡ **High Performance** - Optimized for large-scale data imports  
🎯 **Type-Safe** - Compile-time validation of bulk copy mappings  
📦 **Easy Integration** - Simple attribute-based API  
🔄 **Async/Await Support** - Full async support with cancellation tokens  
🎨 **Clean Code** - Works seamlessly with modern C# patterns (IAsyncEnumerable)

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package Olibos.Data.SqlClient.SqlBulkCopy
```

Or via Package Manager Console:

```powershell
Install-Package Olibos.Data.SqlClient.SqlBulkCopy
```

## Quick Start

### 1. Annotate Your Model

Add the `[SqlBulkCopy]` attribute to any class you want to bulk copy:

```csharp
using Olibos.Data.SqlClient.SqlBulkCopy;

[SqlBulkCopy]
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### 2. Use the Generated Extensions

Build your project, and a source generator automatically creates optimized bulk copy methods:

```csharp
using Microsoft.Data.SqlClient;

await using var connection = new SqlConnection("YOUR_CONNECTION_STRING");
await connection.OpenAsync();

using var bulkCopy = new SqlBulkCopy(connection)
{
    DestinationTableName = "dbo.Products"
};

await bulkCopy.WriteToServerAsync(GetProductsAsync());

async IAsyncEnumerable<Product> GetProductsAsync()
{
    for (int i = 0; i < 10000; i++)
    {
        yield return new Product
        {
            Id = i,
            Name = $"Product {i}",
            Price = 99.99m,
            CreatedAt = DateTime.UtcNow
        };
    }
}
```

## How It Works

This project uses **Roslyn source generators** to:

1. **Detect** - Finds all classes decorated with `[SqlBulkCopy]`
2. **Analyze** - Extracts property information from your models
3. **Generate** - Creates optimized `WriteToServerAsync` extension methods
4. **Integrate** - Seamlessly integrates into your compilation

### Generated Code Example

For a model like:

```csharp
[SqlBulkCopy]
public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
}
```

The generator creates:

```csharp
public static partial class SqlBulkCopyExtensions
{
    public static async Task WriteToServerAsync(
        this SqlBulkCopy sql, 
        IAsyncEnumerable<User> items, 
        CancellationToken cancellationToken = default)
    {
        await using var reader = new global_User(items);
        await sql.WriteToServerAsync(reader, cancellationToken);
    }
    
    private class global_User(
        IAsyncEnumerable<User> enumerable, 
        CancellationToken cancellationToken = default) 
        : BaseAsyncEnumerableReader
    {
        // Implementation details...
    }
}
```

## Project Structure

```
Olibos.Data.SqlClient.SqlBulkCopy/
├── Olibos.Data.SqlClient.SqlBulkCopy/           # Source generator implementation
│   ├── SqlBulkCopySourceGenerator.cs            # Main generator logic
│   └── Olibos.Data.SqlClient.SqlBulkCopy.csproj
├── Olibos.Data.SqlClient.SqlBulkCopy.Sample/    # Usage examples
│   ├── Program.cs
│   ├── Examples.cs
│   └── Model.cs
├── Olibos.Data.SqlClient.SqlBulkCopy.Tests/     # Unit tests
│   └── SourceGeneratorWithAttributesTests.cs
└── README.md
```

## Building & Development

### Prerequisites

- .NET 10.0 SDK or later
- Visual Studio 2022 / Rider / VS Code

### Build

```bash
dotnet build
```

### Run Tests

```bash
dotnet test
```

### Run Sample

```bash
cd Olibos.Data.SqlClient.SqlBulkCopy.Sample
dotnet run
```

## Debugging

The project includes a debug configuration in `Properties/launchSettings.json` for debugging source generators in Visual Studio.

To debug the generator:

1. Open the solution in Visual Studio
2. Set breakpoints in `SqlBulkCopySourceGenerator.cs`
3. Launch using the provided debug profile
4. The sample project will compile and hit your breakpoints

## Technical Details

### Supported Property Types

The generator automatically creates column mappings for:

- Primitives: `int`, `long`, `string`, `bool`, `byte`, `double`, `decimal`, `float`
- Value Types: `DateTime`, `Guid`, `TimeSpan`
- Nullable types: `int?`, `string?`, etc.
- Enums

### Requirements

- Properties must have both public getter and setter
- Classes must be decorated with `[SqlBulkCopy]`
- Classes must be public
- Classes must be in the compilation that references the source generator package

### Performance Characteristics

- **Code Generation Time**: Negligible (happens during build)
- **Runtime Overhead**: Minimal (generated code is optimized)
- **Memory Usage**: Efficient streaming with `IAsyncEnumerable`
- **Throughput**: Limited primarily by database and network, not the generator

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Learning Resources

To learn more about Roslyn source generators:

- [Source Generators Cookbook](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md)
- [Let's Build an Incremental Source Generator With Roslyn](https://youtu.be/azJm_Y2nbAI) - Stefan Pölz
- [Microsoft Source Generators Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)

## FAQ

**Q: Do I need to manually map columns?**  
A: By default, the generator uses positional mapping — properties are mapped to columns in the order they appear. If you need to map by column name or use a different order, you can use the `ColumnMappings` property of `SqlBulkCopy`:

```csharp
using var bulkCopy = new SqlBulkCopy(connection);
bulkCopy.ColumnMappings.Add("sourceColumnName", "destinationColumnName");
await bulkCopy.WriteToServerAsync(items);
```

This allows you to map properties to different column names or reorder them as needed.

**Q: What if I have a large number of properties?**  
A: The generator efficiently handles any number of properties. Performance is determined by your database, not the generator.

## Support

If you encounter issues or have questions:

1. Check the [FAQ](#faq) section
2. Review the [sample project](./Olibos.Data.SqlClient.SqlBulkCopy.Sample)
3. Check existing [GitHub Issues](https://github.com/olibos/Olibos.Data.SqlClient.SqlBulkCopy/issues)
4. Create a new issue with details about your problem
