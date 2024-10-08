using minimal_api.Dominio.Enuns;

namespace MinimalApi.Dominio.ModelViews;

public record AdministradorModeView
{
	public int	Id { get; set; } = default!;
	public string Email { get; set; } = default!;
	public string Perfil { get; set; } = default!;
}
