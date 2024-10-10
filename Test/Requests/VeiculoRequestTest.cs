using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MinimalApi.DTOs;
using Test.Helpers;

namespace Teste.Requests;

[TestClass]
public class VeiculoRequestTest 
{
	[ClassInitialize]
	public static void ClassInit(TestContext testContext)
	{
		Setup.ClassInit(testContext);
	}
	
	[ClassCleanup]
	public static void ClassCleanup()
	{
		Setup.ClassCleanup();
	}
	
	[TestMethod]
	public async Task TestarGetSetPropriedades()
	{
		// Arrange
		var veiculoDTO = new VeiculoDTO
		{
			Nome = "teste",
			Marca = "teste",
			Ano = 1960
		};

		 Setup.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Setup.Token);

		var content = new StringContent(JsonSerializer.Serialize(veiculoDTO), Encoding.UTF8, "application/json");
		
		// Act
		var response = await Setup.client.PostAsync("/veiculos", content);
		var location = response.Headers.Location; 
		var veiculoResponse = await Setup.client.GetAsync(location);
				
		Assert.AreEqual(HttpStatusCode.OK, veiculoResponse.StatusCode);
		var result = await response.Content.ReadAsStringAsync();
		var veiculo = JsonSerializer.Deserialize<VeiculoDTO>(result, new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true
		});

		Assert.IsNotNull(veiculo?.Nome ?? "");
		Assert.IsNotNull(veiculo?.Marca ?? "");
		Assert.IsNotNull(veiculo?.Ano);
	}
}

