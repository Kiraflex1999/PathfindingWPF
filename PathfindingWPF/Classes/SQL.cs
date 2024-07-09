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

        #region SQL
        public List<Node> GetNodes()
        {
            var nodes = new List<Node>();

            string query = "SELECT X, Y FROM dbo.Nodes";

            using (SqlCommand command = new(query, _connection))
            {
                _connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var record = (IDataRecord)reader;

                        nodes.Add(new Node(new Point(Convert.ToDouble(record[1]), Convert.ToDouble(record[2]))));
                    }
                }
                _connection.Close();
            }

            return nodes;
        }

        public List<Node> GetChunkNodes(Chunk chunk)
        {
            var nodes = new List<Node>();

            string query =
                $"SELECT * FROM dbo.Nodes " +
                $"WHERE X BETWEEN {chunk.Point.X} AND {chunk.Point.X + chunk.SizeX} " +
                $"AND Y BETWEEN {chunk.Point.Y} AND {chunk.Point.Y + chunk.SizeX};";

            using (SqlCommand command = new(query, _connection))
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

        internal List<Path> GetPaths(List<Chunk> chunks)
        {
            var paths = new List<Path>();

            foreach (var chunk in chunks)
            {
                foreach (var node in chunk.GetNodes())
                {
                    string query =
                        $"SELECT * FROM dbo.Nodes_Nodes WHERE NodesId1 = " +
                        $"(SELECT Id FROM dbo.Nodes WHERE X = {node.Point.X} AND Y = {node.Point.Y}) " +
                        $"OR NodesId2 = (SELECT Id FROM dbo.Nodes WHERE X = {node.Point.X} AND Y = {node.Point.Y});";

                    using (SqlCommand command = new(query, _connection))
                    {
                        _connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var record = (IDataRecord)reader;

                                paths.Add(new Path((int)record[0], (int)record[1]));
                            }
                        }
                        _connection.Close();
                    }
                }
            }

            return paths;
        }
        #endregion
    }
}
