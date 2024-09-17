using System;
using Microsoft.Data.SqlClient;

Console.WriteLine("Hello, World!");

string s = GetConnectionString();

OpenSqlConnection(s);
Console.WriteLine("Press any key to exit");
Console.ReadLine();

static void OpenSqlConnection(string connectionString)
{
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
        Console.WriteLine("ServerVersion: {0}", connection.ServerVersion);
        Console.WriteLine("State: {0}", connection.State);

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM test.dbo.test";
        var dataReader = command.ExecuteReader();

        while (dataReader.Read())
        {
            Console.WriteLine(dataReader[0]);
        }

        connection.Close();
    }
}

static string GetConnectionString()
{
    // using local sql server with self generated cert so set TrustServerCertificate=True
    //return "Data Source=james-ms\\SQLEXPRESS;Initial Catalog=test;;TrustServerCertificate=True;Integrated Security=SSPI;";
    return "Server=tcp:127.0.0.1,62489;Database=master;User ID=testuser;Password=password123ABC12;TrustServerCertificate=True";
}