# Webcrawler

## Desenvolvimento de um Webcrawler

### Objetivo

Desenvolver um webcrawler capaz de:

1. Acessar o site "https://proxyservers.pro/proxy/list/order/updated/order_dir/desc".

### Passos

#### 1. Acessar o Site

1.1. Acessar o site "https://proxyservers.pro/proxy/list/order/updated/order_dir/desc".

#### 2. Itens

##### 2.1. Extrair Informações

- 2.1.1. Extrair os campos "IP Address", "Port", "Country" e "Protocol" de todas as linhas em todas as páginas disponíveis durante a execução.
- 2.1.2. Salvar o resultado da extração em um arquivo JSON na máquina.
- 2.1.3. Salvar em banco de dados a data de início da execução, data de término da execução, quantidade de páginas, quantidade de linhas extraídas em todas as páginas e o arquivo JSON gerado.
- 2.1.4. Capturar um print (arquivo .html) de cada página acessada.
- 2.1.5. Implementar um webcrawler multithread, limitando a 3 execuções simultâneas.

## Demonstração
![Demonstração](/gif/Demonstracao.gif)
