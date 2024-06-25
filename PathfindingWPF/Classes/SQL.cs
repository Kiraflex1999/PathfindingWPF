using Microsoft.Data.SqlClient;
using System.Data;
using System.Windows;

namespace PathfindingWPF.Classes
{
    internal class SQL
    {
        private SqlConnection _connection;
        private readonly string _connectionString =
            "server=127.0.0.1,1433;" +
            "user id=SA;" +
            "password=Mailo2010;" +
            "initial catalog=PathFinding;" +
            "TrustServerCertificate=True";

        public SQL()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.ConnectionString = _connectionString;
            _connection = new SqlConnection(builder.ConnectionString);
        }

        internal List<Node> GetNodeData()
        {
            List<Node> nodes = new();

            string query = "SELECT Id, X, Y FROM dbo.Nodes";

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                _connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var record = (IDataRecord)reader;

                        nodes.Add(new Node((int)record[0], new Point(Convert.ToDouble(record[1]), Convert.ToDouble(record[2]))));
                    }
                }
                _connection.Close();
            }

            return nodes;
        }

        internal List<Node> GetNeighborNodeData(List<Node> nodes)
        {
            string query = "SELECT NodesId1, NodesId2 FROM dbo.Nodes_Nodes";

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                _connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var record = (IDataRecord)reader;

                        var node = nodes.Where(node => node.Id == (int)record[0]).Single();

                        node.AddNeighborNode(nodes.Where(node => node.Id == (int)record[1]).Single());
                    }
                }
                _connection.Close();
            }

            return nodes;
        }
    }
}
