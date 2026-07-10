using Domain.Entities;

namespace Domain.Tests.EntityTests;

public class UserTests
{
    [Fact]
    public void FullName_WithAllParts_ReturnsConcatenatedString()
    {
        var user = new User { FirstName = "Пётр", LastName = "Петров", MiddleName = "Петрович" };

        var fullName = user.FullName;

        Assert.Equal("Пётр Петров Петрович", fullName);
    }

    [Fact]
    public void FullName_WithOnlyFirstAndLast_IncludesEmptyMiddle()
    {
        var user = new User { FirstName = "Ann", LastName = "Smith", MiddleName = "" };

        var fullName = user.FullName;

        Assert.Equal("Ann Smith ", fullName);
    }

    [Fact]
    public void IsActive_DefaultValue_IsTrue()
    {
        var user = new User();

        Assert.True(user.IsActive);
    }
}
