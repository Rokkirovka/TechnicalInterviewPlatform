using Domain.Enums;

namespace Domain.Tests.EnumTests;

public class EnumTests
{
    [Fact]
    public void InterviewStatus_HasAllValues()
    {
        var values = Enum.GetValues<InterviewStatus>();

        Assert.Contains(InterviewStatus.Scheduled, values);
        Assert.Contains(InterviewStatus.Passed, values);
        Assert.Contains(InterviewStatus.Approved, values);
        Assert.Contains(InterviewStatus.Rejected, values);
        Assert.Contains(InterviewStatus.ToNextStage, values);
        Assert.Equal(5, values.Length);
    }

    [Fact]
    public void ProficiencyLevel_HasAllValues()
    {
        var values = Enum.GetValues<ProficiencyLevel>();

        Assert.Contains(ProficiencyLevel.Beginner, values);
        Assert.Contains(ProficiencyLevel.Middle, values);
        Assert.Contains(ProficiencyLevel.High, values);
        Assert.Equal(3, values.Length);
    }
}
