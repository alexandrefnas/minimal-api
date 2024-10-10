using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Teste.Domain.Sevicos;

[TestClass]
public class VeiculoServicoTest
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
	public void	TestandoSalvarVeiculo()
	{
		// Arrange
		var context = CriarContextoDeTeste();
		context.Database.ExecuteSqlRaw("TRUNCATE TABLE veiculos");
		var veiculo = new Veiculo();		
		veiculo.Nome = "Fiesta 2.0";
		veiculo.Marca = "Ford";
		veiculo.Ano = 2013;
				
		var veiculoServico = new VeiculoServico(context);
		// Act
		veiculoServico.Incluir(veiculo);

		// Assert
		Assert.AreEqual(1, veiculoServico.Todos(1).Count());		
	}
	
	[TestMethod]
	public void	TestandoBuscaPorId()
	{
		// Arrange
		var context = CriarContextoDeTeste();
		context.Database.ExecuteSqlRaw("TRUNCATE TABLE veiculos");
		var veiculo = new Veiculo();		
		veiculo.Nome = "teste";
		veiculo.Marca = "teste";
		veiculo.Ano = 1960;
				
		var veiculoServico = new VeiculoServico(context);
		// Act
		veiculoServico.Incluir(veiculo);

		var veiculoDoBanco = veiculoServico.BuscarPorId(veiculo.Id);

		// Assert
		Assert.AreEqual(1, veiculoDoBanco?.Id);		
	}
}
