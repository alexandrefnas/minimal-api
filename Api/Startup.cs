using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.Dominio.Enuns;
using MinimalApi;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Servicos;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Db;

public class Startup
{
	public IConfiguration Configuration { get; set; } = default!;
	private string key = ""; 

	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
		key = Configuration?.GetSection("Jwt")?.ToString() ?? "";
	}

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddAuthentication(option =>
		{
			option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(option =>
		{
			option.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateLifetime = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
				ValidateIssuer = false,
				ValidateAudience = false
			};
		});

		services.AddAuthorization();
		// fim Token

		services.AddScoped<IAdministradorServico, AdminstradorServico>();
		services.AddScoped<IVeiculoServico, VeiculoServico>();

		services.AddEndpointsApiExplorer(); //Swagger
		services.AddSwaggerGen(options =>
		{
			options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Name = "Authorization",
				Type = SecuritySchemeType.Http,
				Scheme = "bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header,
				Description = "Insira o token JWT aqui:"
			});

			// options.AddSecurityRequirement(); //Swagger // Sem Token
			options.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						}
				},
				new string[] {}
				}
			});
		}); //Swagger

		services.AddDbContext<DbContexto>(options =>
		{
			options.UseMySql(
				Configuration.GetConnectionString("MySql"),
				ServerVersion.AutoDetect(Configuration.GetConnectionString("MySql"))
			);
		});
	}
	
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{	
		app.UseSwagger(); 	//Swagger
		app.UseSwaggerUI();	//Swagger

		app.UseRouting();
		
		app.UseAuthentication(); // Token
		app.UseAuthorization();  // Token


		app.UseEndpoints(endpoints => { 
			#region Home
			endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");

			#endregion

			#region Administradores
			string GerarTokenJwt(Administrador administrador)  // Token
			{
				if (string.IsNullOrEmpty(key)) return string.Empty;
				var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
				var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

				var claims = new List<Claim>()
				{
					new Claim("Email", administrador.Email),
					new Claim("Perfil", administrador.Perfil),
					new Claim(ClaimTypes.Role, administrador.Perfil),
				};
				var token = new JwtSecurityToken(
					claims: claims,
					expires: DateTime.Now.AddDays(1),
					signingCredentials: credentials
				);
				return new JwtSecurityTokenHandler().WriteToken(token);
			}

			endpoints.MapPost("/Administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
			{
				var adm = administradorServico.Login(loginDTO);
				if (adm != null)
				{
					string token = GerarTokenJwt(adm);
					return Results.Ok(new AdministradorLogado
					{
						Email = adm.Email,
						Perfil = adm.Perfil,
						Token = token
					});
				}
				else
					return Results.Unauthorized();
			}).AllowAnonymous().WithTags("Administradores");

			endpoints.MapPost("/Administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
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
			})
			.RequireAuthorization()
			.RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
			.WithTags("Administradores");

			endpoints.MapGet("/Administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
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
			})
			.RequireAuthorization()
			.RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
			.WithTags("Administradores");

			endpoints.MapGet("/Administradores/{id}", ([FromQuery] int id, IAdministradorServico administradorServico) =>
			{
				var administrador = administradorServico.BuscaPorId(id);
				if (administrador == null) return Results.NotFound();
				return Results.Ok(new AdministradorModeView
					{
						Id = administrador.Id,
						Email = administrador.Email,
						Perfil = administrador.Perfil
					});
			})
			.RequireAuthorization()
			.RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
			.WithTags("Administradores");
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

			endpoints.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
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
				return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
			})
			.RequireAuthorization()
			.RequireAuthorization(new AuthorizeAttribute{Roles = "Adm, Editor"})
			.WithTags("Veiculos");

			endpoints.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
			{
				var veiculos = veiculoServico.Todos(pagina);
				
				return Results.Ok(veiculos);
			}).RequireAuthorization().WithTags("Veiculos");

			endpoints.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
			{
				var veiculo = veiculoServico.BuscarPorId(id);
				if (veiculo == null) return Results.NotFound();
				
				return Results.Ok(veiculo);
			}).RequireAuthorization().WithTags("Veiculos");

			endpoints.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
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
			})
			.RequireAuthorization()
			.RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
			.WithTags("Veiculos");

			endpoints.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
			{
				var veiculo = veiculoServico.BuscarPorId(id);

				if (veiculo == null) return Results.NotFound();

				veiculoServico.Apagar(veiculo);
				
				return Results.NoContent();
			})
			.RequireAuthorization()
			.RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
			.WithTags("Veiculos");
			#endregion
		});
	}
}