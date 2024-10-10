namespace MinimalApi.Dominio.ModelViews;

public record VeiculoLogado
{
	public string Nome { get; set; } = default!;
	public string Marca { get; set; } = default!;	
	public int Ano { get; set; } = default!;
}
