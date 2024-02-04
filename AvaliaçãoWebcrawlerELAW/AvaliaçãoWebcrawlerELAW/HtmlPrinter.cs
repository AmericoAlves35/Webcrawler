using AvaliaçãoWebcrawlerELAW;
using System;
using System.IO;

class HtmlPrinter
{
    //private const string basePath = @"C:\Users\xxx\xxx\htmlPages";//caminho da maquina-computador local
    private const string basePath = @"C:\Users\Americo\Desktop\AtividadeElaw\htmlPages";
    private static Logger logger;

    public static void PrintHtml(string html, int pageNumber, string jsonFileName)
    {
        try
        {
            logger = new Logger();
            // Salvar o HTML dentro da pasta
            string filePath = Path.Combine(basePath, $"page_{pageNumber}.html");
            File.WriteAllText(filePath, html);

            // Console.WriteLine($"HTML da página {pageNumber} salvo em: {filePath}");
            string message1 = $"HTML da página {pageNumber} salvo em: {filePath}";
            Console.WriteLine(message1);
           // logger.LogInformation(message1);

        }
        catch (Exception ex)
        {
            string message2=$"Erro ao salvar o HTML: {ex.Message}";
            
            // Console.WriteLine(message2);
            logger.LogError(message2);
        }
    }
}




