namespace Domain.Exceptions.Dto;

public class DtoFieldException(string dtoName, string fieldName, string errorMessage) : ArgumentException;

