using Microsoft.Data.SqlClient;
using PathfindingWPF.Classes.MapObjects;
using System.Data;
using System.Windows;

namespace PathfindingWPF.Classes.Database
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

        #region GetData
        public List<Chunk> GetChunks(double canvasHeight, double canvasWidth)
        {
            var chunks = new List<Chunk>();

            double y = 0;
            int chunkSize = MapData.Instance.GetChunkSizeX();

            while (y < canvasHeight + chunkSize)
            {
                double x = 0;
                while (x < canvasWidth + chunkSize)
                {
                    var chunk = new Chunk(new Point(x, y));
                    x += chunkSize;
                    chunk.AddNode(GetChunkNodes(chunk));
                    chunks.Add(chunk);
                }
                y += chunkSize;
            }

            return chunks;
        }

        private List<Node> GetChunkNodes(Chunk chunk)
        {
            var nodes = new List<Node>();

            string query =
                $"SELECT * FROM dbo.Nodes " +
                $"WHERE X BETWEEN {chunk.Point.X} AND {chunk.Point.X + chunk.SizeX - 1} " +
                $"AND Y BETWEEN {chunk.Point.Y} AND {chunk.Point.Y + chunk.SizeX - 1};";

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

        public List<Path> GetPaths(List<Node> nodes)
        {
            var paths = new List<Path>();

            foreach (var node in nodes)
            {
                string query =
                    $"SELECT * FROM dbo.Nodes_Nodes WHERE NodesId1 = {node.Id} " +
                    $"OR NodesId2 = {node.Id};";

                using (SqlCommand command = new(query, _connection))
                {
                    _connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        IDataRecord? record = null;

                        while (reader.Read())
                        {
                            record = (IDataRecord)reader;

                            if (MapData.Instance.GetNodes().Count < 2)
                            {
                                break;
                            }

                            if (MapData.Instance.GetNodes().Where(n => n.Id == (int)record[0]).ToList().Count == 0 ||
                                MapData.Instance.GetNodes().Where(n => n.Id == (int)record[1]).ToList().Count == 0)
                            {
                                continue;
                            }

                            Node node1 = MapData.Instance.GetNodes().Where(n => n.Id == (int)record[0]).First();
                            Node node2 = MapData.Instance.GetNodes().Where(n => n.Id == (int)record[1]).First();

                            if (paths.Where(p => p.Node1 == node1 && p.Node2 == node2 ||
                                p.Node1 == node2 && p.Node2 == node1).ToList().Count > 0)
                            {
                                continue;
                            }

                            paths.Add(new Path(node1, node2));
                        }
                    }
                    _connection.Close();
                }
            }

            return paths;
        }
        #endregion

        #region AddData
        public void Save(List<Node> nodes, List<Node> removedNodes, List<Path> paths, List<Path> removedPaths)
        {
            SavePaths(paths, removedPaths);
            SaveNodes(nodes, removedNodes);
        }

        private void SavePaths(List<Path> paths, List<Path> removedPaths)
        {
            foreach (var path in paths)
            {
                string query =
                    $"IF NOT EXISTS (SELECT * FROM dbo.Nodes_Nodes " +
                    $"WHERE NodesId1 = {path.Node1.Id} AND NodesId2 = {path.Node2.Id} " +
                    $"OR NodesId1 = {path.Node2.Id} AND NodesId2 = {path.Node1.Id}) " +
                    $"BEGIN " +
                    $"INSERT INTO dbo.Nodes_Nodes VALUES ({path.Node1.Id}, {path.Node2.Id}) " +
                    $"END;";

                _connection.Open();
                using (SqlCommand command = new(query, _connection))
                {
                    command.BeginExecuteNonQuery();
                }
                _connection.Close();
            }

            foreach (var path in removedPaths)
            {
                string query =
                    $"IF EXIST (SELECT * FROM dbo.Nodes_Nodes " +
                    $"WHERE NodesId1 = {path.Node1.Id} AND NodesId2 = {path.Node2.Id} " +
                    $"OR NodesId1 = {path.Node2.Id} AND NodesId2 = {path.Node1.Id}) " +
                    $"BEGIN " +
                    $"DELETE FROM dbo.Nodes_Nodes WHERE NodesId1 = {path.Node1.Id} AND NodesId2 = {path.Node2.Id} " +
                    $"OR NodesId1 = {path.Node2.Id} AND NodesId2 = {path.Node1.Id}) " +
                    $"END;";

                _connection.Open();
                using (SqlCommand command = new(query, _connection))
                {
                    command.BeginExecuteNonQuery();
                }
                _connection.Close();
            }
        }

        private void SaveNodes(List<Node> nodes, List<Node> removedNodes)
        {
            foreach (var node in nodes)
            {
                if (node.Id > 0) { continue; }

                string query = $"INSERT INTO dbo.Nodes (X, Y) VALUES ({node.Point.X}, {node.Point.Y});";

                _connection.Open();
                using (SqlCommand command = new(query, _connection))
                {
                    command.BeginExecuteNonQuery();
                }
                _connection.Close();
            }

            foreach (var node in removedNodes)
            {
                if (node.Id == 0) { continue; }

                string query = $"DELETE FROM dbo.Nodes WHERE Id = {node.Id};";

                _connection.Open();
                using (SqlCommand command = new(query, _connection))
                {
                    command.BeginExecuteNonQuery();
                }
                _connection.Close();
            }
        }
        #endregion
    }
}
