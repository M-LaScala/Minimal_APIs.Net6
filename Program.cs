/* Editar arquivo de projeto -> botão direito no projeto Minimal_APis.Net6
 *
 *<Project Sdk="Microsoft.NET.Sdk"> mudar para <Project Sdk="Microsoft.NET.Sdk.Web">
 *
 * Informa que sera uma aplicação Web
 *
 * <OutputType>Exe</OutputType> Remover pois não sera usado
 *
*/

// Usando o EntityFramework para o banco de dados
using Microsoft.EntityFrameworkCore;

// Gera o builder
var builder = WebApplication.CreateBuilder(args);

// Services - Configurando o banco de dados para memoria local
builder.Services.AddDbContext<ComputerContext>
    (o => o.UseInMemoryDatabase("cComputers"));

// Cria o app com base no builder
var app = builder.Build();

// MapGet ele retorna algo
// MapPost ele recebe algo ( inserção )
// MapPut  ele recebe algo ( editar ) - Editar algo que já existe
// Mapdelet ele recebe algo ( deletar ) - Deleta um registro

/* Exclusão fisica APAGAR o registro ( não é rocomendado )
 * Exclusão logica criar um campo "Cancelado" "Inativo" "Deletado" e marcar como TRUE
 * e tratar de verificar o campo
 */

// Quando chega uma requisição é informado a msg
app.MapGet("/", () => "Ola usuario" );

// O => é uma expressão lambda ( Pesquisar dps )

// Endpoints
app.MapGet("/Computers", async(ComputerContext context) => 
                                await context.Computers.ToListAsync());

app.MapGet("/Computers/{id}", async(int id, ComputerContext context) => 
                                await context.Computers.FirstOrDefaultAsync(a=>a.Id == id));

app.MapPost("/Computers", async(Computer computer, ComputerContext context, HttpContext httpContext) 
    =>
    {

        context.Computers.Add(computer);
        await context.SaveChangesAsync();

        // No lugar de retornar o objeto é melhor retornar o resultado 
        //return computer;

        return Results.Created($"/computers/{computer.Id}", computer);

    });

app.MapPut("/computers/{id}", async(Computer computer, ComputerContext context) 
    =>
    {
        context.Entry(computer).State = EntityState.Modified;
        await context.SaveChangesAsync();

        return computer;
    });

app.MapDelete("/computers/{id}", async (int id, ComputerContext context)
    =>
    {
        var computer = await context.Computers.FirstOrDefaultAsync(a => a.Id == id);
        context.Computers.Remove(computer);
        await context.SaveChangesAsync();

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

/* Vamos usar o entit framework para gravar os dados pelo oq entendi
* Este provedor de banco de dados permite que o Entity Framework Core seja usado com um banco de dados em memória. 
* O banco de dados na memória pode ser útil para teste 
* O banco de dados na memória é projetado apenas para teste.
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
 * Colar a url https://localhost:44373 <- No caso a minha
 * colocar no modo post
 * Body -> raw -> JSON
 * 
 * 
*/

// Exemplo requisiçao
/*
{
    "name":"Mackbook",
    "brand":"Apple"
}
*/