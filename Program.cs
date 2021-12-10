/* Editar arquivo de projeto -> botão direito no projeto Minimal_APis.Net6
 *
 *<Project Sdk="Microsoft.NET.Sdk"> mudar para <Project Sdk="Microsoft.NET.Sdk.Web">
 *
 * Informa que sera uma aplicação Web
 *
 * <OutputType>Exe</OutputType> Remover pois não sera usado
 *
*/

var builder = WebApplication.CreateBuilder(args);
// Services 

var app = builder.Build();

app.MapGet("/", () => "Ola usuario" );

app.Run();