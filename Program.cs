using Microsoft.EntityFrameworkCore;
using MinimalApi.Infraestrutura.Db;
using MinimalApi.DTOs;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Servicos;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Entidades;
using minimal_api.Dominio.Enuns;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdminstradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer(); //Swagger
builder.Services.AddSwaggerGen();	//Swagger

builder.Services.AddDbContext<DbContexto>(options =>
{
	options.UseMySql(
		builder.Configuration.GetConnectionString("mysql"),
		ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
	);
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");

#endregion

#region Administradores
app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
	if (administradorServico.Login(loginDTO) != null)
		return Results.Ok("login com sucesso!");
	else
		return Results.Unauthorized();
}).WithTags("Administradores");

app.MapPost("/Administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
{
	var validacao = new ErrosDeValidacao
	{
		Mensagens = new List<string>()
	};

	if (string.IsNullOrEmpty(administradorDTO.Email)) validacao.Mensagens.Add("Email não pode ser vazio");
	if (string.IsNullOrEmpty(administradorDTO.Senha)) validacao.Mensagens.Add("Senha não pode ser vazia");
	if (administradorDTO.Perfil == null) validacao.Mensagens.Add("Perfil não pode ser vazio");

	if (validacao.Mensagens.Count > 0) return Results.BadRequest(validacao);

	var administrador = new Administrador
	{
		Email = administradorDTO.Email,
		Senha = administradorDTO.Senha,
		Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
	};

	administradorServico.Incluir(administrador);

	return Results.Created($"/administrador/{administrador.Id}", new AdministradorModeView
		{
			Id = administrador.Id,
			Email = administrador.Email,
			Perfil = administrador.Perfil
		});
}).WithTags("Administradores");

app.MapGet("/Administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
{
	var adms = new List<AdministradorModeView>();
	var administradores = administradorServico.Todos(pagina);
	foreach(var adm in administradores)
	{
		adms.Add(new AdministradorModeView
		{
			Id = adm.Id,
			Email = adm.Email,
			Perfil = adm.Perfil
		});
	}
	return Results.Ok(adms);
}).WithTags("Administradores");

app.MapGet("/Administradores/{id}", ([FromQuery] int id, IAdministradorServico administradorServico) =>
{
	var administrador = administradorServico.BuscaPorId(id);
	if (administrador == null) return Results.NotFound();
	return Results.Ok(new AdministradorModeView
		{
			Id = administrador.Id,
			Email = administrador.Email,
			Perfil = administrador.Perfil
		});
}).WithTags("Administradores");
#endregion

#region Veiculos

ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
{
	var validacao = new ErrosDeValidacao{
		Mensagens = new List<string>()
	};

	if (string.IsNullOrEmpty(veiculoDTO.Nome)) validacao.Mensagens.Add("O nome não pode ser vazio!");
	if (string.IsNullOrEmpty(veiculoDTO.Marca)) validacao.Mensagens.Add("A Marca não pode ficar em branco!");
	if (veiculoDTO.Ano < 1950) validacao.Mensagens.Add("Veículo muito antigo, aceito somente anos superior a 1950!");

	return validacao;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{	
	var validacao = validaDTO(veiculoDTO);
	if (validacao.Mensagens.Count > 0) return Results.BadRequest(validacao);
	
	var veiculo = new Veiculo
	{
		Nome = veiculoDTO.Nome,
		Marca = veiculoDTO.Marca,
		Ano = veiculoDTO.Ano
	};
	veiculoServico.Incluir(veiculo);
	return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
{
	var veiculos = veiculoServico.Todos(pagina);
	
	return Results.Ok(veiculos);
}).WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
	var veiculo = veiculoServico.BuscarPorId(id);
	if (veiculo == null) return Results.NotFound();
	
	return Results.Ok(veiculo);
}).WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{	
	var veiculo = veiculoServico.BuscarPorId(id);
	if (veiculo == null) return Results.NotFound();

	var validacao = validaDTO(veiculoDTO);
	if (validacao.Mensagens.Count > 0) return Results.BadRequest(validacao);

	veiculo.Nome = veiculoDTO.Nome;
	veiculo.Marca = veiculoDTO.Marca;
	veiculo.Ano = veiculoDTO.Ano;
	
	veiculoServico.Atualizar(veiculo);
	return Results.Ok(veiculo);
}).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
	var veiculo = veiculoServico.BuscarPorId(id);

	if (veiculo == null) return Results.NotFound();

	veiculoServico.Apagar(veiculo);
	
	return Results.NoContent();
}).WithTags("Veiculos");


#endregion

#region APP
app.UseSwagger(); 	//Swagger
app.UseSwaggerUI();	//Swagger
app.Run();
#endregion

// Parou em Configurando token JWT no projeto