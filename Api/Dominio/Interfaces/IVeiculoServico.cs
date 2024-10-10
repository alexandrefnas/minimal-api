using MinimalApi.Dominio.Entidades;

namespace MinimalApi.Dominio.Interfaces;

public interface IVeiculoServico
{
	List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null);
	Veiculo? BuscarPorId(int id);
	void Incluir(Veiculo veiculo);
	void Atualizar(Veiculo veiculo);	
	void Apagar(Veiculo veiculo);
}
