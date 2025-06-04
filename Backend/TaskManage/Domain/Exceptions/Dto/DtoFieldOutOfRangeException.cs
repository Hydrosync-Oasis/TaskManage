namespace Domain.Exceptions.Dto
{
    public class DtoFieldOutOfRangeException(string dtoName, string fieldName)
        : DtoFieldException(dtoName, fieldName, "该字段的值不存在或超出范围");
}
