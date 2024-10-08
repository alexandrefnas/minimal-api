using Microsoft.EntityFrameworkCore;
using MinimalApi.Infraestrutura.Db;
using MinimalApi.DTOs;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Servicos;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdminstradorServico>();

builder.Services.AddDbContext<DbContexto>(options =>
{
	options.UseMySql(
		builder.Configuration.GetConnectionString("mysql"),
		ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
	);
});
var app = builder.Build();

app.MapGet("/", () => "Olá mundo!");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
	if (administradorServico.Login(loginDTO) != null)
		return Results.Ok("login com sucesso!");
	else
		return Results.Unauthorized();
});

app.Run();
 