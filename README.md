# Local Database MCP Tools

This VS Code extension provides Model Context Protocol (MCP) tools for interacting with local SQL Server databases. It enables you to execute database operations directly from VS Code using natural language commands.

[![Demo Video of working MCP]()](https://www.youtube.com/watch?v=R4FGtwKSPcA)

## Features

The extension provides the following database operations:

- **Test Connection**: Simple echo test to verify the MCP connection
- **List Tables**: View all tables in a specified database
- **List Columns**: Inspect the structure of any table, including column names, types, and nullable status
- **Execute Queries**: Run SELECT queries against your local database

## Prerequisites

- Visual Studio Code
- SQL Server LocalDB installed
- .NET 9.0 or later
- Model Context Protocol (MCP) extension for VS Code

## Setup

1. Ensure you have SQL Server LocalDB installed on your machine
2. Clone this repository
3. Open the project in Visual Studio Code
4. Build the project using:
   ```
   dotnet build
   ```

## Usage

After installing and setting up the extension, you can use the following commands in VS Code:

1. **Test Connection**:
   ```
   Test "your message"
   ```
   This will echo back your message to confirm the connection is working.

2. **List Tables**:
   ```
   ListTables "YourDatabaseName"
   ```
   This will display all tables in the specified database.

3. **List Columns**:
   ```
   ListColumns "YourDatabaseName" "SchemaName.TableName"
   ```
   Example: `ListColumns "quizdb" "dbo.GameSessions"`
   This will show all columns, their data types, and nullable status for the specified table.

4. **Execute Query**:
   ```
   ExecuteQuery "YourDatabaseName" "YOUR_SELECT_QUERY"
   ```
   Example: `ExecuteQuery "quizdb" "SELECT * FROM dbo.GameSessions"`
   Note: Only SELECT queries are allowed for security reasons.

## Security Notes

- The tool only allows SELECT queries to prevent unauthorized data modifications
- Uses Windows Authentication (Trusted Connection) for database access
- TLS encryption is enabled by default

## Connection String

The tool uses LocalDB with the following connection string format:
```
Server=(localdb)\MSSQLLocalDB;Database={databaseName};Trusted_Connection=True;TrustServerCertificate=True;
```

## Dependencies

- Microsoft.Data.SqlClient
- ModelContextProtocol.Server (MCP library)

## Contributing

Feel free to submit issues and enhancement requests!

## License

[Add your chosen license here]

