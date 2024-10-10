using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.DTOs;

namespace Test.Mocks
{
    public class VeiculoServicoMock : IVeiculoServico
    {
		private static List<Veiculo> veiculos = new List<Veiculo>()
		{
			new Veiculo
			{
				Id = 1,
				Nome = "Fiesta 2.0",
				Marca = "Ford",
				Ano = 2013
			},
			new Veiculo
			{
				Id = 2,
				Nome = "Gol 1.0",
				Marca = "VW",
				Ano = 2010
			},
		};
        	
        public void Apagar(Veiculo veiculo)
        {
			veiculos.Remove(veiculo);	
        }

        public void Atualizar(Veiculo veiculo)
        {
            throw new NotImplementedException();
        }

        public Veiculo? BuscarPorId(int id)
        {
            return veiculos.Find(a => a.Id == id);
        }

        public void Incluir(Veiculo veiculo)
        {
			veiculo.Id = veiculos.Count() + 1;
			veiculos.Add(veiculo);			
        }

        public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
        {
			return veiculos;
        }
    }
}