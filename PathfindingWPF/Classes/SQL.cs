using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Text;
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

        internal List<Node> GetNeighborNodeData(List<Node> nodes)
        {
            string query = "SELECT NodesId1, NodesId2 FROM dbo.Nodes_Nodes";

            using (SqlCommand command = new(query, _connection))
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

        internal void SaveNodes(List<Node> nodes)
        {
            StringBuilder sb = new();

            sb.Append("INSERT INTO dbo.Nodes (X, Y) VALUES ");
            bool save = false;

            foreach (Node node in nodes)
            {
                if (node.Id != 0) { continue; }
                save = true;
                sb.Append($"({(int)node.Point.X}, {(int)node.Point.Y}), ");
            }

            sb.Replace(",", ";", sb.Length - 2, 1);

            if (save)
            {
                RunSaveSqlCommand(sb);
            }
        }

        internal void SaveNeighborNodes(List<Node> nodes)
        {
            bool nodeId = false;
            bool neighborId = false;

            foreach (Node node in nodes)
            {
                nodeId = node.Id != 0;

                foreach (Node neighbor in node.GetNeighborNodes())
                {
                    neighborId = neighbor.Id != 0;

                    StringBuilder sb = new();

                    sb.Append("IF NOT EXISTS (SELECT * FROM Nodes_Nodes WHERE ");

                    if (nodeId && neighborId)
                    {
                        SaveNeighborNode(node.Id, neighbor.Id, sb);
                    }
                    else if (!nodeId && neighborId)
                    {
                        int id = GetNodeIdFromDatabase(node.Point);
                        if (id == 0) { throw new Exception("id is 0"); }
                        else { SaveNeighborNode(id, neighbor.Id, sb); };
                    }
                    else if (nodeId && !neighborId)
                    {
                        int id = GetNodeIdFromDatabase(neighbor.Point);
                        if (id == 0) { throw new Exception("id is 0"); }
                        else { SaveNeighborNode(node.Id, id, sb); };
                    }
                    else if (!nodeId && !neighborId)
                    {
                        int id1 = GetNodeIdFromDatabase(node.Point);
                        int id2 = GetNodeIdFromDatabase(neighbor.Point);
                        if (id1 == 0 || id2 == 0) { throw new Exception("id is 0"); }
                        else { SaveNeighborNode(id1, id2, sb); };
                    }
                    else
                    {
                        throw new Exception("Something went wrong in SaveNeighborNodes()");
                    }
                }
            }
        }

        private int GetNodeIdFromDatabase(Point point)
        {
            int id = 0;
            using (SqlCommand command = new($"SELECT Id FROM Nodes WHERE X = {point.X} AND Y = {point.Y};", _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var record = (IDataRecord)reader;

                        id = (int)record[0];
                    }
                }
            }
            return id;
        }

        private void SaveNeighborNode(int id1, int id2, StringBuilder sb)
        {
            sb.Append($"NodesId1 = {id1} AND NodesId2 = {id2}) INSERT INTO Nodes_Nodes VALUES ({id1}, {id2});");
            RunSaveSqlCommand(sb);
        }

        private void RunSaveSqlCommand(StringBuilder sb)
        {
#if DEBUG
            Debug.WriteLine(sb.ToString());
#endif
            using (SqlCommand command = new(sb.ToString(), _connection))
            {
                _connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
#if DEBUG
                Debug.WriteLine($"Number of rows affected: {rowsAffected}");
#endif
                _connection.Close();
            }
        }
    }
}
