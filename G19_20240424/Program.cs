using DatabaseHelper.SQL;

namespace G19_20240424
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var database = new Database("Server=.;Database=NorthWind;Integrated Security=True; TrustServerCertificate = true;");
            try
            {
                database.OpenConnection();
                database.BeginTransaction();

                for (int i = 0; i < 10; i++)
                {
                    database.ExecuteNonQuery($"INSERT INTO Categories (CategoryName) VALUES ('Nika{i}')");
                    //if (i == 5)
                    //{
                    //    throw new Exception("Test exception");
                    //}
                }

                database.CommitTransaction();
            }
            catch (Exception ex)
            {
                database.RollbackTransaction();
                Console.WriteLine(ex.Message);
            }
            finally
            {
                database.CloseConnection();
            }

            ////database.ExecuteNonQuery("INSERT INTO Categories (Name) VALUES ('Nika')");
            ////test reader
            //database.OpenConnection();
            //using SqlDataReader reader = database.ExecuteReader("SELECT * FROM Categories");
            //while (reader.Read())
            //{
            //    Console.WriteLine(reader["CategoryName"]);
            //}
        }
    }
}
