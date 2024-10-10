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
	public Administrador Incluir(Administrador administrador)
	{
		_contexto.Administradores.Add(administrador);
		_contexto.SaveChanges();

		return administrador;
	}
	
	public Administrador? BuscaPorId(int id)
	{
		return _contexto.Administradores.Where(v => v.Id == id).FirstOrDefault();
	}

	public List<Administrador> Todos(int? pagina)
	{
		var query = _contexto.Administradores.AsQueryable();		
		int itensPorPagina = 10;
		if (pagina != null)
		{
			query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
		}
		return query.ToList();
	}

}
