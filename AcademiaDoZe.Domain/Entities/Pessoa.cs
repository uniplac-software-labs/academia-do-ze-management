// Pedro Henrique dos Santos

using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities;

public abstract class Pessoa : Entity
{
    public string Nome { get; protected set; }
    public Cpf Cpf { get; protected set; }
    public DateOnly DataNascimento { get; protected set; }
    public Telefone Telefone { get; protected set; }
    public Email Email { get; protected set; }
    public Endereco Endereco { get; protected set; }
    public Senha Senha { get; protected set; }
    public Arquivo Foto { get; protected set; }

    protected Pessoa(
        int id,
        string nome,
        Cpf cpf,
        DateOnly dataNascimento,
        Telefone telefone,
        Email email,
        Endereco endereco,
        Senha senha,
        Arquivo foto)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome é obrigatório.", nameof(nome));

        if (dataNascimento > DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("A data de nascimento não pode estar no futuro.", nameof(dataNascimento));

        Nome = nome;
        Cpf = cpf ?? throw new ArgumentNullException(nameof(cpf));
        DataNascimento = dataNascimento;
        Telefone = telefone ?? throw new ArgumentNullException(nameof(telefone));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Endereco = endereco ?? throw new ArgumentNullException(nameof(endereco));
        Senha = senha ?? throw new ArgumentNullException(nameof(senha));
        Foto = foto ?? throw new ArgumentNullException(nameof(foto));
    }
}