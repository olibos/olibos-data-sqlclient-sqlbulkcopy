namespace Olibos.Data.SqlClient.SqlBulkCopy.Sample;

using System;

[SqlBulkCopy]
public class Model
{
    public int Id { get; set; }
    
    public string? Name { get; set; }
    
    public DateTime Date { get; set; }
}

