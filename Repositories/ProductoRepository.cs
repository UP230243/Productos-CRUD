namespace BackendProductos.Repositories
{
    using System.Data;
    using MySqlConnector;
    using BackendProductos.Models;

    public class ProductoRepository : IProductoRepository
    {
        private readonly string _connectionString;

        public ProductoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                                ?? throw new InvalidOperationException("La cadena de conexión no fue encontrada.");
        }

        private IDbConnection CrearConexion()
        {
            return new MySqlConnection(_connectionString);
        }

        public List<Producto> ObtenerTodos()
        {
            var productos = new List<Producto>();
            using var connection = CrearConexion();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Nombre, Precio, Stock FROM Productos";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                productos.Add(new Producto
                {
                    Id = reader.GetInt32("Id"),
                    Nombre = reader.GetString("Nombre"),
                    Precio = reader.GetDecimal("Precio"),
                    Stock = reader.GetInt32("Stock")
                });
            }
            return productos;
        }

        public Producto? ObtenerPorId(int id)
        {
            using var connection = CrearConexion();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Nombre, Precio, Stock FROM Productos WHERE Id = @Id";
            
            command.Parameters.Add(CrearParametro(command, "@Id", id));

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Producto
                {
                    Id = reader.GetInt32("Id"),
                    Nombre = reader.GetString("Nombre"),
                    Precio = reader.GetDecimal("Precio"),
                    Stock = reader.GetInt32("Stock")
                };
            }
            return null;
        }

        public int Insertar(Producto producto)
        {
            using var connection = CrearConexion();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Productos (Nombre, Precio, Stock) VALUES (@Nombre, @Precio, @Stock); SELECT LAST_INSERT_ID();";

            command.Parameters.Add(CrearParametro(command, "@Nombre", producto.Nombre));
            command.Parameters.Add(CrearParametro(command, "@Precio", producto.Precio));
            command.Parameters.Add(CrearParametro(command, "@Stock", producto.Stock));

            var idGenerado = Convert.ToInt32(command.ExecuteScalar());
            return idGenerado;
        }

        public bool Actualizar(Producto producto)
        {
            using var connection = CrearConexion();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE Productos SET Nombre = @Nombre, Precio = @Precio, Stock = @Stock WHERE Id = @Id";

            command.Parameters.Add(CrearParametro(command, "@Id", producto.Id));
            command.Parameters.Add(CrearParametro(command, "@Nombre", producto.Nombre));
            command.Parameters.Add(CrearParametro(command, "@Precio", producto.Precio));
            command.Parameters.Add(CrearParametro(command, "@Stock", producto.Stock));

            int filasAfectadas = command.ExecuteNonQuery();
            return filasAfectadas > 0;
        }

        public bool Eliminar(int id)
        {
            using var connection = CrearConexion();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Productos WHERE Id = @Id";

            command.Parameters.Add(CrearParametro(command, "@Id", id));

            int filasAfectadas = command.ExecuteNonQuery();
            return filasAfectadas > 0;
        }

        private IDbDataParameter CrearParametro(IDbCommand command, string nombre, object valor)
        {
            var param = command.CreateParameter();
            param.ParameterName = nombre;
            param.Value = valor ?? DBNull.Value;
            return param;
        }
    }
}
