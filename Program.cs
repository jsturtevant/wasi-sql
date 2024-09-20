using System;
using Microsoft.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

public class App
{
    static int Main(string[] args)
    {
        string address = "";
        string additionalParams = "";
        if (args.Length >= 1)
        {
            address = args[0];
        }
        if (args.Length >= 2)
        {
            additionalParams = args[1];
        }

        var task = MainAsync(address, additionalParams);
        if (!OperatingSystem.IsWasi())
        {
            Console.WriteLine("Hello!");
            task.Wait();
        }
        else
        {
            Console.WriteLine("Hello from WASI!");
            while (!task.IsCompleted)
            {
                WasiEventLoop.Dispatch();
            }
            var exception = task.Exception;
            if (exception is not null)
            {
                throw exception;
            }
        }

        Console.WriteLine("goodbye");
        return 0;
    }

    static async Task MainAsync(string addressString, string additionalParams)
    {
        string s = GetConnectionString(addressString, additionalParams);
        await OpenSqlConnection(s);
    }


    static async Task OpenSqlConnection(string connectionString)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            Console.WriteLine("ServerVersion: {0}", connection.ServerVersion);
            Console.WriteLine("State: {0}", connection.State);

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM HelloWorld.dbo.HelloWorldTable";
            var dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                Console.WriteLine(dataReader[0]);
            }

            connection.Close();
        }
    }

    static string GetConnectionString(string addressString, string additionalParams)
    {
        if (String.IsNullOrEmpty(addressString))
        {
            addressString ="127.0.0.1,1433";
        }

        if (String.IsNullOrEmpty(additionalParams))
        {
            additionalParams = "TrustServerCertificate=True";
        }

        // using local sql server with self generated cert so set TrustServerCertificate=True
        //return "Data Source=james-ms\\SQLEXPRESS;Initial Catalog=test;;TrustServerCertificate=True;Integrated Security=SSPI;";
        return $"Server=tcp:{addressString};Database=HelloWorld;User ID=sa;Password=YourStrong@Passw0rd!;Pooling=false;{additionalParams}";
    }

    internal static class WasiEventLoop
    {
        internal static void Dispatch()
        {
            CallDispatchWasiEventLoop((Thread)null!);

            [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "DispatchWasiEventLoop")]
            static extern void CallDispatchWasiEventLoop(Thread t);
        }
    }
}
