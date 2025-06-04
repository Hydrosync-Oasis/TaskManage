namespace Domain.Exceptions.Dto;

public class DtoFieldNullException(string dtoName, string fieldName) : DtoFieldException(dtoName, fieldName, "字段值不能为空");

