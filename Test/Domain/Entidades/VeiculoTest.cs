using MinimalApi.Dominio.Entidades;

namespace Teste.Domain.Entidades;

[TestClass]
public class VeiculoTest
{
	[TestMethod]
	public void TestarGetSetPropriedades()
	{	
		// Arrange
		var veiculo = new Veiculo();

		// Act
		veiculo.Id = 1;
		veiculo.Nome = "CarroTest";
		veiculo.Marca = "teste";
		veiculo.Ano = 1960;

		// Assert
		Assert.AreEqual(1, veiculo.Id);
		Assert.AreEqual("CarroTest", veiculo.Nome);
		Assert.AreEqual("teste", veiculo.Marca);
		Assert.AreEqual(1960, veiculo.Ano);
	}
}