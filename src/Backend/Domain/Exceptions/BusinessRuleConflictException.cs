namespace Domain.Exceptions;

public class BusinessRuleConflictException(string message) : Exception(message);