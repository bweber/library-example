using System.Data;
using Microsoft.Data.SqlClient;

namespace Library.AcceptanceTests.Utilities
{
    public static class ConnectionFactory
    {
        private const string ConnectionString = "Server=localhost,1533;Database=Library;User Id=sa;Password=password123!;";

        public static IDbConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}