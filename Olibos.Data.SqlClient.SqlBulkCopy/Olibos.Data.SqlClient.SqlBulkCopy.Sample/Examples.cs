namespace Olibos.Data.SqlClient.SqlBulkCopy.Sample;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

public class Examples
{
    public async Task Import()
    {
        await using var connection = new SqlConnection();
        await connection.OpenAsync();
        using var bulk = new SqlBulkCopy(connection);
        bulk.DestinationTableName = "[dummy].[Model]";
        bulk.ColumnMappings.Add(nameof(Model.Id), nameof(Model.Id));
        bulk.ColumnMappings.Add(nameof(Model.Name), nameof(Model.Name));
        bulk.ColumnMappings.Add(nameof(Model.Date), nameof(Model.Date));
        await bulk.WriteToServerAsync(LoadData());
    }

    private async IAsyncEnumerable<Model> LoadData()
    {
        yield return new Model { Id = 1, Name = "John Doe", Date = DateTime.Now };
    }
}