using APBD_ZAO_CW5.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace APBD_ZAO_CW5.DAL
{
    public class DbService : IDbService
    {
        private readonly string connectionString = @"Data Source=db-mssql;Initial Catalog=s20289;Integrated Security=True";

        public async Task<int> AddProductToWarehouseAsync(ProductWarehouse productWarehouse)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand();

            command.Connection = connection;
            await connection.OpenAsync();

            command.CommandText = "SELECT TOP 1 [Order].IdOrder FROM [Order] " +
                "LEFT JOIN Product_Warehouse ON [Order].IdOrder = Product_Warehouse.IdOrder " +
                "WHERE [Order].IdProduct = @IdProduct " +
                "AND [Order].Amount = @Amount " +
                "AND Product_Warehouse.IdProductWarehouse IS NULL " +
                "AND [Order].CreatedAt < @CreatedAt";

            command.Parameters.AddWithValue("IdProduct", productWarehouse.IdProduct);
            command.Parameters.AddWithValue("Amount", productWarehouse.Amount);
            command.Parameters.AddWithValue("CreatedAt", productWarehouse.CreatedAt);

            var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows) throw new Exception("Wrong parameter");

            await reader.ReadAsync();
            int idOrder = int.Parse(reader["IdOrder"].ToString());
            await reader.CloseAsync();

            command.Parameters.Clear();

            command.CommandText = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";
            command.Parameters.AddWithValue("IdProduct", productWarehouse.IdProduct);

            reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows) throw new Exception("Product with this Id does not exist!");

            await reader.ReadAsync();
            double price = double.Parse(reader["Price"].ToString());
            await reader.CloseAsync();

            command.Parameters.Clear();

            command.CommandText = "SELECT IdWarehouse FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
            command.Parameters.AddWithValue("IdWarehouse", productWarehouse.IdWarehouse);

            reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows) throw new Exception("Warehouse with this Id does not exist!");

            await reader.CloseAsync();
            command.Parameters.Clear();


            var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
            command.Transaction = transaction;

            try
            {
                command.CommandText = "UPDATE [Order] SET FulfilledAt = @CreatedAt WHERE IdOrder = @IdOrder";
                command.Parameters.AddWithValue("CreatedAt", productWarehouse.CreatedAt);
                command.Parameters.AddWithValue("IdOrder", idOrder);

                int rowsUpdated = await command.ExecuteNonQueryAsync();

                if (rowsUpdated < 1) throw new Exception("No rows updated");

                command.Parameters.Clear();

                command.CommandText = "INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) " +
                    $"VALUES(@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Amount*{price}, @CreatedAt)";

                command.Parameters.AddWithValue("IdWarehouse", productWarehouse.IdWarehouse);
                command.Parameters.AddWithValue("IdProduct", productWarehouse.IdProduct);
                command.Parameters.AddWithValue("IdOrder", idOrder);
                command.Parameters.AddWithValue("Amount", productWarehouse.Amount);
                command.Parameters.AddWithValue("CreatedAt", productWarehouse.CreatedAt);

                int rowsInserted = await command.ExecuteNonQueryAsync();

                if (rowsInserted < 1) throw new Exception("No rows inserted");

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw new Exception("Internal server problem!");
            }

            command.Parameters.Clear();

            command.CommandText = "SELECT TOP 1 IdProductWarehouse FROM Product_Warehouse ORDER BY IdProductWarehouse DESC";

            reader = await command.ExecuteReaderAsync();

            await reader.ReadAsync();
            int idProductWarehouse = int.Parse(reader["IdProductWarehouse"].ToString());
            await reader.CloseAsync();

            await connection.CloseAsync();

            return idProductWarehouse;
        }

      
    }
}
