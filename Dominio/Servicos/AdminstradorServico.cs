using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Db;

namespace MinimalApi.Dominio.Servicos;

public class AdminstradorServico : IAdministradorServico
{
	private readonly DbContexto _contexto;
	public AdminstradorServico(DbContexto contexto)
	{
		_contexto = contexto;
	}

	public Administrador? Login(LoginDTO loginDTO)
	{
		var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
		return adm;	   
	}
}
