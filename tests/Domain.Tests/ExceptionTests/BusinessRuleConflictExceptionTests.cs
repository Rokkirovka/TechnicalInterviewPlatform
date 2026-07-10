using Domain.Exceptions;

namespace Domain.Tests.ExceptionTests;

public class BusinessRuleConflictExceptionTests
{
    [Fact]
    public void Constructor_SetsMessage()
    {
        var message = "test message";

        var exception = new BusinessRuleConflictException(message);

        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void IsException()
    {
        var exception = new BusinessRuleConflictException("msg");

        Assert.IsAssignableFrom<Exception>(exception);
    }
}
