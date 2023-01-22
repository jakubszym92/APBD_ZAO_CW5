using APBD_ZAO_CW5.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace APBD_ZAO_CW5.DAL
{
    public class DbProcedureService : IDbProcedureService
    {
        private readonly string connectionString = @"Data Source=db-mssql;Initial Catalog=s20289;Integrated Security=True";
        public async Task<int> AddProductToWarehouseAsync(ProductWarehouse productWarehouse)
        {
            int idProductWarehouse = 0;

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand("AddProductToWarehouse", connection);

            var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
            command.Transaction = transaction;

            try
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("IdProduct", productWarehouse.IdProduct);
                command.Parameters.AddWithValue("IdWarehouse", productWarehouse.IdWarehouse);
                command.Parameters.AddWithValue("Amount", productWarehouse.Amount);
                command.Parameters.AddWithValue("CreatedAt", productWarehouse.CreatedAt);

                await connection.OpenAsync();
                int rowsChanged = await command.ExecuteNonQueryAsync();

                if (rowsChanged < 1) throw new Exception("No rows changed");

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw new Exception();
            }

            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT TOP 1 IdProductWarehouse FROM Product_Warehouse ORDER BY IdProductWarehouse DESC";

            using var reader = await command.ExecuteReaderAsync();

            await reader.ReadAsync();
            if (await reader.ReadAsync())
                idProductWarehouse = int.Parse(reader["IdProductWarehouse"].ToString());
            await reader.CloseAsync();

            await connection.CloseAsync();

            return idProductWarehouse;
        }
    }
}
