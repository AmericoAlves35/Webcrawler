using System;

namespace AvaliaçãoWebcrawlerELAW
{
    class Program
    {
        private static Logger logger;

        static void Main()
        {
            logger = new Logger();

            logger.LogInformation(""); // Adiciona uma linha em branco
            logger.LogInformation("Iniciando execução...");

            SiteAccses dataExtractor = new SiteAccses();
            _ = dataExtractor.DownoadoPG();
            
            Console.ReadLine();
           
            logger.LogInformation("...Fim da execução");
            logger.LogInformation(""); // Adiciona uma linha em branco

        }
    }
}
