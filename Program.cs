using System;
using Microsoft.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

public class App
{
    static int Main(string[] args)
    {

        var task = MainAsync(args[0]);
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

    static async Task MainAsync(string addressString)
    {
        string s = GetConnectionString(addressString);
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

    static string GetConnectionString(string addressString)
    {
        // using local sql server with self generated cert so set TrustServerCertificate=True
        //return "Data Source=james-ms\\SQLEXPRESS;Initial Catalog=test;;TrustServerCertificate=True;Integrated Security=SSPI;";
        return $"Server=tcp:{addressString};Database=HelloWorld;User ID=sa;Password=YourStrong@Passw0rd!;Pooling=false;TrustServerCertificate=True;Encrypt=false";
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
