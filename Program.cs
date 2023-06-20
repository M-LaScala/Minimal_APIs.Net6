/* 
 * Editar arquivo de projeto -> botão direito no projeto Minimal_APis.Net6
 *
 * <Project Sdk="Microsoft.NET.Sdk"> mudar para <Project Sdk="Microsoft.NET.Sdk.Web">
 *
 * Informa que sera uma aplicação Web
 *
 * <OutputType>Exe</OutputType> Remover pois não sera usado
 *
*/

// Usando o EntityFramework para o banco de dados
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;

// Gera o builder
var builder = WebApplication.CreateBuilder(args);

// Services - Configurando o banco de dados para memoria local
builder.Services.AddDbContext<ComputerContext>
    (o => o.UseInMemoryDatabase("Computers"));

// Configurando o swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cria o app com base no builder
var app = builder.Build();

// MapGet ele retorna algo
// MapPost ele recebe algo ( inserção )
// MapPut  ele recebe algo ( editar ) - Editar algo que já existe
// Mapdelet ele recebe algo ( deletar ) - Deleta um registro

/* 
 * Exclusão fisica APAGAR o registro ( não é rocomendado )
 * Exclusão logica criar um campo "Cancelado" "Inativo" "Deletado" e marcar como TRUE
 * e tratar de verificar o campo
 */

// Definindo o endpoint do swagger
app.MapSwagger();
// Interface do swagger
app.UseSwaggerUI();

// MapGet é um método de extensão que é usado para mapear uma rota HTTP GET.
// No exemplo, estamos mapeando a rota para o caminho raiz ("/") da aplicação.
// .ExcludeFromDescription(); Remove a documentação do swagger
string msg = "Olá usuário, para acessar o Swagger, utilize a rota /swagger.\nPara visualizar a mensagem de boas-vindas, utilize a rota /html.";
app.MapGet("/", () => msg).ExcludeFromDescription();


// Inicio criação deu uma 'Pagina' Html 
// A expressão Results.Extensions.HtmlA é específica para retornar uma resposta no formato HTML.
// Essa extensão é útil quando você deseja retornar uma página HTML completa como resultado de uma ação de controlador.
// Ela permite que você construa facilmente uma resposta HTML personalizada e retorne-a para o cliente.

string titulo = "Bem-vindo, usuário! É uma surpresa vê-lo por aqui.";
string conteudoChatGPT = "<p>Aqui está o conteúdo HTML gerado pelo Chat GPT!</p>";
string versaoChat = "<p>Versão atual do chat: 1.0.0.</p>";

string conteudoHtml = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='UTF-8'>
            <title>Bem-vindo!</title>
            <style>
                .container {{
                    display: flex;
                    flex-direction: column;
                    align-items: center;
                    justify-content: center;
                    height: 100vh;
                    text-align: center;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h1>{titulo}</h1>
                {conteudoChatGPT}
                {versaoChat}
            </div>
        </body>
        </html>
    ";

app.MapGet("/html", () => Results.Extensions.Html(conteudoHtml)).ExcludeFromDescription();

// Os '=>' são uma expressão lambda.

/* 
* As expressões lambda são uma ferramenta poderosa para escrever código conciso e legível em C#.
* Elas são amplamente utilizadas em programação assíncrona, LINQ (Language Integrated Query), manipulação de eventos, entre outros cenários onde funções anônimas são necessárias.
*/

// Funções Anônimas:
/* 
* Significa que você pode definir uma função sem fornecer um nome explícito. 
* Isso é útil quando você precisa de uma função simples que será usada apenas em um contexto específico e não precisa ser definida em outro lugar.
*/

// Uso com Métodos de Ordem Superior:
/* 
* Por exemplo Where, Select, OrderBy, entre outros, para realizar operações em coleções de objetos. 
* Elas permitem que você especifique a lógica que será aplicada a cada elemento da coleção de forma concisa e flexível.
*/

// Async e Await
/*
 * A palavra-chave async é usada para definir um método como assíncrono.
 * 
 * A palavra-chave await é usada para aguardar a conclusão de uma operação assíncrona. 
 * Ela é usada dentro de um método assíncrono para indicar o ponto em que o fluxo de execução deve esperar até que a operação assíncrona seja concluída antes de continuar.
*/


// Demais Endpoints
app.MapGet("/Computers", async (ComputerContext context) =>
                                await context.Computers.ToListAsync())
    .Produces<List<Computer>>(StatusCodes.Status200OK);

app.MapGet("/Computers/{id}", async(int id, ComputerContext context) => 
                                await context.Computers.FirstOrDefaultAsync(a=>a.Id == id));

app.MapPost("/Computers", async(Computer computer, ComputerContext context, HttpContext httpContext) 
    =>
    {

        context.Computers.Add(computer);
        await context.SaveChangesAsync();

        // No lugar de retornar o objeto é melhor retornar o resultado 
        //return computer;

        return Results.Created($"/Computers/{computer.Id}", computer);

    });

app.MapPut("/Computers/{id}", async(Computer computer, ComputerContext context) 
    =>
    {
        context.Entry(computer).State = EntityState.Modified;
        await context.SaveChangesAsync();

        return computer;
    });

app.MapDelete("/Computers/{id}", async (int id, ComputerContext context)
    =>
    {
        var computer = await context.Computers.FirstOrDefaultAsync(a => a.Id == id);
        
        if (computer != null && computer.Id == id)
        {
            context.Computers.Remove(computer);
            await context.SaveChangesAsync();
        }
        else
        {
            return Results.BadRequest("O id informado não foi encontrado.");
        }

        /* Results.NoContent() retorna um resultado HTTP com o status 204 No Content, 
         * que indica que a requisição foi processada com sucesso, mas não há conteúdo para ser retornado.
        */
        return Results.NoContent();
    });

// Inicia o app
app.Run();

// Classe computador
// Ponto de ? na frente do nome da variavel para mostrar que e nullable 
public class Computer
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Brand { get; set; }

}

/* Vamos usar o entit framework para gravar os dados
* Este provedor de banco de dados permite que o Entity Framework Core seja usado com um banco de dados em memória. 
* O banco de dados na memória pode ser útil para testes
*
* Microsoft.EntityFrameworkCore.InMemory
* Botão direito no projeto gerenciar pacotes nugget
*/

// Data context faz a conexão com o banco de dados

public class ComputerContext : DbContext
{

    public ComputerContext(DbContextOptions options) : base(options) { }
    public DbSet<Computer> Computers {  get; set; }

}

/* Anotações sobre o Postman
 * Serve para enviar requisições para uma url 
 * 
 * Colar a url https://localhost:44373
 * colocar no modo post
 * Body -> raw -> JSON
 * 
*/

// Exemplo requisiçao via postman
/*
{
    "name":"Mackbook",
    "brand":"Apple"
}
*/

// Como usar o swagger - Projeto - botão direito - pacotes nuget ( não pode estar em execução )
// Swashbuckle.AspNetCore 
/*
 *Ele interpreta o JSON do Swagger a fim de criar uma experiência rica e 
 *personalizável para descrever a funcionalidade da API Web.
*/

static class ResultsExtensions
{
    public static IResult Html(this IResultExtensions resultExtensions, string html)
    {
        return new HtmlResult(html);
    }
}

class HtmlResult : IResult
{

    private string _html;

    public HtmlResult(string html)
    {
        _html = html;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.ContentType = MediaTypeNames.Text.Html;
        httpContext.Response.ContentLength = Encoding.UTF8.GetByteCount(_html);
        return httpContext.Response.WriteAsync(_html);
    }
}