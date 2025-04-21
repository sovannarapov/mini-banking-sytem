using Application.Interfaces;

namespace Application.Services;

public class GuidGenerator : IGuidGenerator
{
    public Guid NewGuid() => Guid.NewGuid();
}
