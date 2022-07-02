using Dapper;
using ProductManager.Database;
using ProductManager.Models;
using Microsoft.Data.Sqlite;

namespace ProductManager.Repositories;
class ProductRepository
{
    private DatabaseConfig databaseConfig;

    public ProductRepository(DatabaseConfig databaseConfig)
    {
        this.databaseConfig = databaseConfig;
    }

    // Insere um produto na tabela
    public Product Save(Product product)
    {
        var connection = new SqliteConnection(databaseConfig.ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText = "INSERT INTO Products VALUES($id, $name, $price, $active);";
        command.Parameters.AddWithValue("$id", product.Id);
        command.Parameters.AddWithValue("$name", product.Name);
        command.Parameters.AddWithValue("$price", product.Price);
        command.Parameters.AddWithValue("$active", product.Active);

        command.ExecuteNonQuery();
        connection.Close();

        return product;
    }

    // Deleta um produto na tabela
    public void Delete(int id)
    {
        var connection = new SqliteConnection(databaseConfig.ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Products WHERE id = $id;";

        command.Parameters.AddWithValue("$id", id);

        command.ExecuteNonQuery();
    }

    // Habilita um produto
    public void Enable(int id)
    {
        using var connection = new SqliteConnection(databaseConfig.ConnectionString);
        connection.Open();

        connection.Execute("UPDATE Products SET active=@active Where id=@id", new { @id = id, @active = true });
    }

    // Desabilita um produto
    public void Disable(int id)
    {
        using var connection = new SqliteConnection(databaseConfig.ConnectionString);
        connection.Open();

        connection.Execute("UPDATE Products SET active=@active Where id=@id", new { @id = id, @active = false });
    }

    // Retorna todos os produtos
    public List<Product> GetAll()
    {
        var connection = new SqliteConnection(databaseConfig.ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, name, price, active FROM Products;";

        var reader = command.ExecuteReader();

        var products = new List<Product>();

        while (reader.Read())
        {
            products.Add(new Product(reader.GetInt32(0), reader.GetString(1), reader.GetDouble(2), reader.GetBoolean(3)));
        }

        connection.Close();

        return products;
    }

    public bool ExistsById(int id)
    {
        using var conn = new SqliteConnection(databaseConfig.ConnectionString);
        conn.Open();

        var exists = conn.ExecuteScalar<bool>("SELECT COUNT(id) FROM Products WHERE id=@id", new { @id = id });
        return exists;
    }

    public List<Product> GetAllWithPriceBetween(double initialPrice, double endPrice)
    {
        using var connection = new SqliteConnection(databaseConfig.ConnectionString);
        connection.Open();
        var products = connection.Query<Product>("SELECT * FROM Products WHERE price > @initialPrice AND price < @endPrice", new { initialPrice = initialPrice, endPrice = endPrice }).ToList();
        return products;
    }

    public List<Product> GetAllWithPriceHigherThan(double price)
    {
        using var connection = new SqliteConnection(databaseConfig.ConnectionString);
        connection.Open();
        var products = connection.Query<Product>("SELECT * FROM Products WHERE price > @price", new { price = price }).ToList();
        return products;
    }

    public List<Product> GetAllWithPriceLowerThan(double price)
    {
        using var connection = new SqliteConnection(databaseConfig.ConnectionString);
        connection.Open();
        var products = connection.Query<Product>("SELECT * FROM Products WHERE price < @price", new { price = price }).ToList();
        return products;
    }

    public double GetAveragePrice()
    {
        using var connection = new SqliteConnection(databaseConfig.ConnectionString);
        connection.Open();
        double average = connection.ExecuteScalar<double>("SELECT AVG(price) FROM Products");
        return average;
    }
}
