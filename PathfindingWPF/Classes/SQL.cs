using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace PathfindingWPF.Classes
{
    internal class SQL
    {
        public void Try()
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                builder.ConnectionString = "server=127.0.0.1,1433;user id=SA;password=Mailo2010;initial catalog=PathFinding;TrustServerCertificate=True";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    string sql = "SELECT Id, X, Y FROM dbo.Nodes";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var record = (IDataRecord)reader;

                                Debug.WriteLine($"{record[0]} {record[1]} {record[2]}");
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Debug.WriteLine(e.ToString());
            }
        }
    }
}
