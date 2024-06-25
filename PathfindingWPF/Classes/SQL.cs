using Microsoft.Data.SqlClient;
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
                    Debug.WriteLine("\nQuery data example:");
                    Debug.WriteLine("=========================================\n");

                    String sql = "SELECT name, collation_name FROM sys.databases";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Debug.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
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
