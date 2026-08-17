// Pedro Henrique dos Santos
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Repositories;

public interface ILogradouroRepository : IRepository<Logradouro>
{
    Task<Logradouro?> ObterPorCep(Cep cep, CancellationToken cancellationToken = default);
    Task<bool> CepJaExiste(Cep cep, int? id = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Logradouro>> ObterPorCidade(string cidade, CancellationToken cancellationToken = default);
    Task<IEnumerable<Logradouro>> ObterPorBairro(string cidade, string bairro, CancellationToken cancellationToken = default);
}