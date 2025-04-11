using ModelContextProtocol.Server;
using System.ComponentModel;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Data;

namespace LocalDBMCP
{
[McpServerToolType]
public static class LocalDBTool
{    private static string GetConnectionString(string databaseName) =>
        $"Server=(localdb)\\MSSQLLocalDB;Database={databaseName};Trusted_Connection=True;TrustServerCertificate=True;";

    [McpServerTool, Description("Echoes the message back to the client.")]
    public static string Test(string message) => $"Hello from Ram: {message}";

    [McpServerTool, Description("Lists all tables in the specified database")]
    public static async Task<string> ListTables(
        [Description("Name of the database")] string databaseName)
    {
        using var connection = new SqlConnection(GetConnectionString(databaseName));
        await connection.OpenAsync();
        
        var tables = await connection.GetSchemaAsync("Tables");
        var sb = new StringBuilder();
        
        foreach (DataRow row in tables.Rows)
        {
            sb.AppendLine($"Table: {row["TABLE_SCHEMA"]}.{row["TABLE_NAME"]}");
        }
        
        return sb.ToString();
    }

    [McpServerTool, Description("Lists all columns and their types for the specified table")]
    public static async Task<string> ListColumns(
        [Description("Name of the database")] string databaseName,
        [Description("Full name of the table (e.g. dbo.Users)")] string tableName)
    {
        using var connection = new SqlConnection(GetConnectionString(databaseName));
        await connection.OpenAsync();

        var cmd = new SqlCommand(
            @"SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
              FROM INFORMATION_SCHEMA.COLUMNS 
              WHERE TABLE_SCHEMA = @schema 
              AND TABLE_NAME = @table
              ORDER BY ORDINAL_POSITION", connection);

        var parts = tableName.Split('.');
        cmd.Parameters.AddWithValue("@schema", parts[0]);
        cmd.Parameters.AddWithValue("@table", parts[1]);

        var sb = new StringBuilder();
        using var reader = await cmd.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            var length = reader["CHARACTER_MAXIMUM_LENGTH"] == DBNull.Value 
                ? "" 
                : $"({reader["CHARACTER_MAXIMUM_LENGTH"]})";
            
            sb.AppendLine($"Column: {reader["COLUMN_NAME"]} | Type: {reader["DATA_TYPE"]}{length} | Nullable: {reader["IS_NULLABLE"]}");
        }

        return sb.ToString();
    }

    [McpServerTool, Description("Executes a SQL query and returns the results")]
    public static async Task<string> ExecuteQuery(
        [Description("Name of the database")] string databaseName,
        [Description("SQL query to execute (SELECT queries only)")] string query)
    {
        using var connection = new SqlConnection(GetConnectionString(databaseName));
        await connection.OpenAsync();

        if (!query.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
        {
            return "Error: Only SELECT queries are allowed for security reasons.";
        }

        var cmd = new SqlCommand(query, connection);
        var sb = new StringBuilder();
        
        using var reader = await cmd.ExecuteReaderAsync();
        
        // Add column headers
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (i > 0) sb.Append(" | ");
            sb.Append(reader.GetName(i));
        }
        sb.AppendLine("\n" + new string('-', sb.Length));

        // Add data rows
        while (await reader.ReadAsync())
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (i > 0) sb.Append(" | ");
                sb.Append(reader[i]);
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}

};
