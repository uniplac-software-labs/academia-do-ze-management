// Pedro Henrique dos Santos

namespace AcademiaDoZe.Domain.Entities;

public abstract class Entity
{
    public int Id { get; protected set; }

    protected Entity(int id = 0)
    {
        if (id < 0)
            throw new ArgumentOutOfRangeException(nameof(id), "O Id não pode ser negativo.");

        Id = id;
    }
}