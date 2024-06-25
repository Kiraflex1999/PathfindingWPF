using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Windows;

namespace PathfindingWPF.Classes
{
    internal class SQL
    {
        public List<Node>? GetData()
        {
            List<Node>? nodes = null;

            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.ConnectionString = "server=127.0.0.1,1433;user id=SA;password=Mailo2010;initial catalog=PathFinding;TrustServerCertificate=True";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    string query = "SELECT Id, X, Y FROM dbo.Nodes";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            nodes = new();

                            while (reader.Read())
                            {
                                var record = (IDataRecord)reader;

                                nodes.Add(new Node((int)record[0], new Point(Convert.ToDouble(record[1]), Convert.ToDouble(record[2]))));
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Debug.WriteLine(e.ToString());
            }

            return nodes;
        }
    }
}
