namespace Domain.Exceptions.Domain
{
    public class EntityNotFoundException(string entityName, object? key) : Exception($"找不到键为{key}的实体{entityName}");
}
