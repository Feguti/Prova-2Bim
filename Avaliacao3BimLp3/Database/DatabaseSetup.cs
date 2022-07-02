using Microsoft.Data.Sqlite;
using Dapper;

namespace ProductManager.Database;

class DatabaseSetup
{
    private DatabaseConfig databaseConfig;

    public DatabaseSetup(DatabaseConfig databaseConfig)
    {
        this.databaseConfig = databaseConfig;
        CreateProductTable();
    }

    public void CreateProductTable()
    {
        var connection = new SqliteConnection(databaseConfig.ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText = @"
        CREATE TABLE IF NOT EXISTS Products(
            id int not null primary key,
            name varchar(100) not null,
            price double not null,
            active boolean not null
    );
";
        command.ExecuteNonQuery();
        connection.Close();
    }
}