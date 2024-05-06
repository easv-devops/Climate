using api.helpers;
using Flyway.net;
using MySqlConnector;

namespace tests.WebSocket;

public class FlywayDbTestRebuilder
{
    public static void ExecuteMigrations()
    {
        try
        {
            // Opret forbindelse til databasen
            using (var connection = new MySqlConnection(Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString())))
            {
                DropAllTables(connection);

                // Hent en liste over SQL-filer i migrationsmappen
                string[] sqlFiles = Directory.GetFiles("../../../../../db/sql", "*.sql");
            
                connection.Open();
            
                // Udfør versionsfiler
                ExecuteVersionFiles(connection, sqlFiles);

                // Udfør repeatable-filer
                ExecuteRepeatableFiles(connection, sqlFiles);
                
                connection.Close();
            }

            Console.WriteLine("DB Migration completed successfully.");
        }
        catch (Exception ex)
        {
            throw new Exception("Error during database migration: " + ex.Message);
        }
    }


    private static void DropAllTables(MySqlConnection connection)
     {
         string databaseName = "mydb";

         try
         {
             // Opret forbindelse til en indbygget database, f.eks. "mysql"
             string systemDatabaseName = "";
             string connectionString = Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString().Replace(databaseName, systemDatabaseName));
             using (var systemConnection = new MySqlConnection(connectionString))
             {
                 systemConnection.Open();

                 // SQL-kommando for at slette den ønskede database
                 string dropDatabaseQuery = $"DROP DATABASE IF EXISTS {databaseName};";

                 // SQL-kommando for at oprette en ny database med samme navn
                 string createDatabaseQuery = $"CREATE DATABASE {databaseName};";

                 // Udfør DROP DATABASE-kommandoen
                 using (var dropDatabaseCommand = new MySqlCommand(dropDatabaseQuery, systemConnection))
                 {
                     dropDatabaseCommand.ExecuteNonQuery();
                 }

                 // Udfør CREATE DATABASE-kommandoen
                 using (var createDatabaseCommand = new MySqlCommand(createDatabaseQuery, systemConnection))
                 {
                     createDatabaseCommand.ExecuteNonQuery();
                 }

                 // Luk forbindelsen til den indbyggede database
                 systemConnection.Close();
             }
         }
         catch (Exception ex)
         {
             throw new Exception("Error during database operations: " + ex.Message);
         }

         // Genopret forbindelsen til den nye database
         connection.Open(); // Åbn forbindelsen før du skifter database
         connection.ChangeDatabase(databaseName);
         connection.Close();
     }
    
   private static void ExecuteVersionFiles(MySqlConnection connection, string[] sqlFiles)
   {
       // Filtrer SQL-files for Version scripts
       var versionFiles = sqlFiles.Where(file => Path.GetFileName(file).StartsWith("V")).OrderBy(file => file);

       // Execute Version 
       foreach (string sqlFile in versionFiles)
       {
           string sqlQuery = File.ReadAllText(sqlFile);

           using (var command = new MySqlCommand(sqlQuery, connection))
           {
               command.ExecuteNonQuery();
           }
       }
   }

   private static void ExecuteRepeatableFiles(MySqlConnection connection, string[] sqlFiles)
   {
       // Filtrer SQL-filer, der starter med "r" (repeatable files)
       var repeatableFiles = sqlFiles.Where(file => Path.GetFileName(file).StartsWith("R")).OrderBy(file => file);

       // Iterer gennem hver repeatable SQL-fil og udfør den mod databasen
       foreach (string sqlFile in repeatableFiles)
       {
           string sqlQuery = File.ReadAllText(sqlFile);

           using (var command = new MySqlCommand(sqlQuery, connection))
           {
               command.ExecuteNonQuery();
           }
       }
            
   } 
}


