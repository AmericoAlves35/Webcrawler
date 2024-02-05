using System;
using MySql.Data.MySqlClient;

namespace AvaliaçãoWebcrawlerELAW
{
    class ConnectionMySql
    {
        //private const string connectionString = "Server=servidor;Database=databaseName;User ID=yourId;Password=pass;";
        private const string connectionString = "Server=localhost;Database=dadoswebcrawler;User ID=root;Password=852040am;";
        private static Logger logger;
        public void MyConnection(int totalPages, int totalRows, string jsonFilePath, DateTime startDate, DateTime endDate)
        {
            logger = new Logger();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    
                    string message1 = "Conexão com banco de dados bem-sucedida!";
                    Console.WriteLine(message1);
                    logger.LogInformation(message1);

                    // Chamada do método para inserir dados
                    InsertExecutionInfo(connection, startDate, endDate, totalPages, totalRows, jsonFilePath);

                  
                    string message2 = "Dados salvos no banco de dados!";
                    Console.WriteLine(message2);
                    logger.LogInformation(message2);
                }
                catch (Exception ex)
                {
                  
                    string message3 = $"Erro ao salvar no banco de dados: {ex.Message}";
                    Console.WriteLine(message3);
                    logger.LogError(message3);
                }
            }
        }

        private void InsertExecutionInfo(MySqlConnection connection, DateTime startDate, DateTime endDate, int totalPages, int totalRows, string jsonFilePath)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = connection;

                    //data de término após a extração das informações das páginas
                   
                    cmd.CommandText = "INSERT INTO ExecutionInfo (StartDate, EndDate, TotalPages, TotalRows, JsonFilePath) " +
                                      "VALUES (@StartDate, @EndDate, @TotalPages, @TotalRows, @JsonFilePath)";

                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    cmd.Parameters.AddWithValue("@TotalPages", totalPages);
                    cmd.Parameters.AddWithValue("@TotalRows", totalRows);
                    cmd.Parameters.AddWithValue("@JsonFilePath", jsonFilePath);

                    cmd.ExecuteNonQuery();//é usado para executar uma instrução SQL que não retorna dados, como uma instrução INSERT, UPDATE, DELETE ou qualquer outra instrução SQL que não seja uma consulta.
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();

                string message4 = $"Erro ao inserir dados: {ex.Message}";
                Console.WriteLine(message4);
                logger.LogError(message4);

            }
        }
    }
}
