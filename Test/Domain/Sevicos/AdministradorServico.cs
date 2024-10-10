using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Teste.Domain.Sevicos;

[TestClass]
public class AdministradorServicoTest
{
	private DbContexto CriarContextoDeTeste()
	{
		//Configurar o ConfigurationBuilder
		var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);		
		var builder = new ConfigurationBuilder()
		.SetBasePath(path ?? Directory.GetCurrentDirectory())
		.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
		.AddEnvironmentVariables();

		var configuration = builder.Build();

		return new DbContexto(configuration);	
	}
	
	[TestMethod]
	public void	TestandoSalvarAdministrador()
	{
		// Arrange
		var context = CriarContextoDeTeste();
		context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");
		var adm = new Administrador();		
		adm.Email = "teste@teste.com";
		adm.Senha = "teste";
		adm.Perfil = "Adm";

		
		var administradorServico = new AdminstradorServico(context);
		// Act
		administradorServico.Incluir(adm);

		// Assert
		Assert.AreEqual(1, administradorServico.Todos(1).Count());		
	}
	
	[TestMethod]
	public void	TestandoBuscaPorId()
	{
		// Arrange
		var context = CriarContextoDeTeste();
		context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");
		var adm = new Administrador();		
		adm.Email = "teste@teste.com";
		adm.Senha = "teste";
		adm.Perfil = "Adm";

		
		var administradorServico = new AdminstradorServico(context);
		// Act
		administradorServico.Incluir(adm);
		var admDoBanco = administradorServico.BuscaPorId(adm.Id);

		// Assert
		Assert.AreEqual(1, admDoBanco?.Id);
		
	}
}
