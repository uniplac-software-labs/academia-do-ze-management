// Pedro Henrique dos Santos
namespace AcademiaDoZe.Domain.Exceptions;

public sealed class DomainException(string message) : Exception(message)
{
}