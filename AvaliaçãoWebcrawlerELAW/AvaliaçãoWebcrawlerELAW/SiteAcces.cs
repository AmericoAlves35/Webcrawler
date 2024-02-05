using AvaliaçãoWebcrawlerELAW;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

class SiteAccses
{

    // //private const string basePath = @"C:\xxx\xxx\xxx\extracaoJSON"; //local para salvar o arquivo JSON
    // private const string basePath = @"C:\Users\Americo\Desktop\AtividadeElaw\extracaoJSON";
    ////private const string htmlPagesPath = @"C:\Users\Americo\Desktop\AtividadeElaw\htmlPages"; local para salvar o arquivo de print html provisorio
    // private const string htmlPagesPath = @"C:\Users\Americo\Desktop\AtividadeElaw\htmlPages";
    // private const string baseUrl = "https://proxyservers.pro/proxy/list/order/updated/order_dir/desc";
    // static string fileName2 = "";

    // private SemaphoreSlim semaphore = new SemaphoreSlim(3); // Limite de 3 execuções simultâneas
    // private List<ProxyInformation> allProxies = new List<ProxyInformation>();

    private const string basePath = @"C:\Users\Americo\Desktop\AtividadeElaw\extracaoJSON";
    private const string htmlPagesPath = @"C:\Users\Americo\Desktop\AtividadeElaw\htmlPages";
  
    private const string baseUrl = "https://proxyservers.pro/proxy/list/order/updated/order_dir/desc";
    private static string fileName2 = "";

    private SemaphoreSlim semaphore = new SemaphoreSlim(3);
    private List<ProxyInformation> allProxies = new List<ProxyInformation>();
    private static Logger logger;
       
    public async Task DownoadoPG()
    {
        logger = new Logger();
        bool isSiteAvailable = await IsSiteAvailable(baseUrl);

        if (isSiteAvailable)
        {
            string message1 ="O site está disponível para consulta. Iniciando o processo de extração...";
            Console.WriteLine(message1);
            logger.LogInformation(message1);

            int totalPages = await GetTotalPages(baseUrl);

            DateTime startDate = DateTime.Now; // Data de início

            // Ajuste para iterar por todas as páginas
            var tasks = new List<Task>();

            for (int i = 1; i <= totalPages; i++)
            {
                tasks.Add(ProcessPageAsync(i));
            }

            // Aguarde todas as tarefas serem concluídas
            await Task.WhenAll(tasks);

            DateTime endDate = DateTime.Now; // Data de término após a extração de todas as páginas

            // Salvar em um arquivo JSON sequencial
            int fileSequence = 1;
            SaveToJsonFile(fileSequence);

            // Incrementar a sequência se o arquivo já existir
            while (File.Exists(Path.Combine(basePath, $"all_proxies_{fileSequence + 1}.json")))
            {
                fileSequence++;
                SaveToJsonFile(fileSequence);
            }
            string message2 = "Processo de extração concluído com sucesso.";
            Console.WriteLine(message2);
            logger.LogInformation(message2);

            // Salvar no banco de dados
            ConnectionMySql saveToDatabase = new ConnectionMySql();
            saveToDatabase.MyConnection(totalPages, allProxies.Count, Path.Combine(basePath, fileName2), startDate, endDate);

            // Criar uma pasta com o nome do JSON dentro de htmlPages e mover os arquivos HTML
            CreateAndMoveHtmlFiles(fileName2);

         
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            string message3= "O site não está disponível para consulta. Verifique a conexão ou a URL do site.";
            Console.WriteLine(message3);
            Console.ResetColor(); // Isso restaura a cor padrão do console
            logger.LogWarning(message3);
            
            
        }
    }

    private async Task<bool> IsSiteAvailable(string url)
    {
        logger = new Logger();
        try
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                           
                Console.ForegroundColor = ConsoleColor.Green;
                string message4=$"Código de status da resposta HTTP: {(int)response.StatusCode} - {response.StatusCode}";
                Console.WriteLine(message4);
                Console.ResetColor(); // Isso restaura a cor padrão do console
                logger.LogInformation(message4);
                
                return response.IsSuccessStatusCode;

            }
        }
        catch (Exception ex)
        {
            string message5= $"Erro ao verificar a disponibilidade do site: {ex.Message}";
            Console.WriteLine(message5);
            logger.LogError(message5);
            
            return false;
        }
    }


    private void CreateAndMoveHtmlFiles(string jsonFilePath)
    {
      
        try
        {
            // Obter o nome do JSON sem a extensão
            string jsonFileName = Path.GetFileNameWithoutExtension(jsonFilePath);

            // Criar uma nova pasta dentro de htmlPages com o nome do JSON
            string folderPath = Path.Combine(htmlPagesPath, jsonFileName);
            Directory.CreateDirectory(folderPath);

                        

            // Mover os arquivos HTML para a nova pasta
            for (int i = 1; i <= allProxies.Count; i++)
            {
                string sourceFilePath = Path.Combine(htmlPagesPath, $"page_{i}.html");
                string destinationFilePath = Path.Combine(folderPath, $"page_{i}.html");

                // Verificar se o arquivo de origem existe
                if (File.Exists(sourceFilePath))
                {
                    File.Move(sourceFilePath, destinationFilePath);
                    //Console.WriteLine($"Arquivo HTML {i} movido para: {destinationFilePath}");
                }
                else
                { 
                    string message6 = $"Arquivo HTML {i} não encontrado em: {sourceFilePath}";
                    
                    Console.WriteLine(message6);
                    //logger.LogError(message6);
                    break;
                }
            }

            string message7 = $"Pasta com arquivos print.html criada: {folderPath}";

            Console.WriteLine(message7);
            logger.LogInformation(message7);
        }
        catch (Exception ex)
        {
            string message8 = $"Erro ao criar pasta e mover arquivos HTML: {ex.Message}";
                     
            Console.WriteLine(message8);
            logger.LogError(message8);
        }
    }

    private async Task ProcessPageAsync(int pageNumber)
    {
      
        await semaphore.WaitAsync();

        try
        {
            string url = $"{baseUrl}/page/{pageNumber}";
            var html = await DownloadHtmlAsync(url, pageNumber);

            // HTML da página, extrair as informações.
            var proxyList = ExtractProxyInformation(html);

            // Adicionar à lista acumulada
            allProxies.AddRange(proxyList);

            //lista de proxies.
            foreach (var proxy in proxyList)
            {
                Console.WriteLine($"IP Address: {proxy.IpAddress}, Port: {proxy.Port}, Country: {proxy.Country}, Protocol: {proxy.Protocol}");
            }
        }
        finally
        {
            semaphore.Release();
        }
    }

    private static async Task<string> DownloadHtmlAsync(string url, int pageNumber)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // Salvar o HTML em um arquivo usando a classe HtmlPrinter
                string html = await response.Content.ReadAsStringAsync();
                HtmlPrinter.PrintHtml(html, pageNumber, fileName2);

                return html;
            }
            catch (Exception ex)
            {
                string message9 = $"Erro ao baixar HTML: {ex.Message}";
                Console.WriteLine(message9);
                logger.LogError(message9);
                return string.Empty;
            }
        }
    }


    private List<ProxyInformation> ExtractProxyInformation(string html)
    {
        var proxyList = new List<ProxyInformation>();
        var document = new HtmlDocument();
        document.LoadHtml(html);

        // Ajuste o seletor para capturar as linhas da tabela dentro da div com a classe 'table-responsive'
        var rows = document.DocumentNode.SelectNodes("//div[@class='table-responsive']/table/tbody/tr");

        if (rows != null)
        {
            foreach (var row in rows)
            {
                var columns = row.SelectNodes("td");
                if (columns != null && columns.Count >= 7)
                {
                    var ipAddress = columns[1].InnerText.Trim(); // Segunda coluna contém o IP Address
                    //var port = columns[2].InnerText.Trim(); // Terceira coluna contém o Port
                    var portNode = columns[2].SelectSingleNode("span[@class='port']");
                    var port = portNode?.InnerText.Trim();//Terceira coluna contém o Port

                    var country = columns[3].InnerText.Trim(); // Quarta coluna contém o Country
                    var protocol = columns[6].InnerText.Trim(); // Sétima coluna contém o Protocol

                    var proxyInfo = new ProxyInformation
                    {
                        IpAddress = ipAddress,
                        Port = port,
                        Country = country,
                        Protocol = protocol
                    };

                    proxyList.Add(proxyInfo);
                }
            }
        }

        return proxyList;
    }


    private void SaveToJsonFile(int fileSequence)
    {
        try
        {
            string fileName = $"all_proxies_{fileSequence}.json";
            string filePath = Path.Combine(basePath, fileName);

            // Verificar se o arquivo já existe
            int count = 1;
            while (File.Exists(filePath))
            {
                count++;
                fileName = $"all_proxies_{fileSequence}_{count}.json";
                filePath = Path.Combine(basePath, fileName);
            }
            fileName2 = fileName;

            // Serializar e salvar no arquivo
            string json = JsonConvert.SerializeObject(allProxies, Formatting.Indented);
            File.WriteAllText(filePath, json);

            string message10=$"Lista de proxies criados em arquivo .JSON salvo em: {filePath}";  
            Console.WriteLine(message10);
            logger.LogInformation(message10);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao salvar o JSON: {ex.Message}");
        }
    }

    private static async Task<int> GetTotalPages(string baseUrl)
    {
        var html = await DownloadHtmlAsync(baseUrl, 1);

        var document = new HtmlDocument();
        document.LoadHtml(html);

        // Uso da classe 'pagination' para obter os elementos de páginação.
        var paginationNode = document.DocumentNode.SelectSingleNode("//ul[contains(@class, 'pagination')]");

        if (paginationNode != null)
        {
            // Capturar o último elemento filho que não tem a classe 'disabled'.
            var lastPageNode = paginationNode.SelectSingleNode("li[not(@class='page-item disabled')][last()]/a");

            if (lastPageNode != null && int.TryParse(lastPageNode.InnerText, out int totalPages))
            {
                return totalPages;
            }
        }

        return 1; // Se não for possível obter o número total de páginas há pelo menos uma página.
    }
}
