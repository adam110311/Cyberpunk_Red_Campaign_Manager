using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Cyberpunk_Campaign.DataAcess
{
    public class SqlConnectionFactory
    {
        private readonly string _connectionString;
        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }
        public System.Data.SqlClient.SqlConnection CreateConnection()
        {
            return new System.Data.SqlClient.SqlConnection(_connectionString);
        }
    }
}
