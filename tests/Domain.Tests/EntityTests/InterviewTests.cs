using Domain.Entities;
using Domain.Enums;

namespace Domain.Tests.EntityTests;

public class InterviewTests
{
    [Fact]
    public void StatusAsString_WhenScheduled_ReturnsScheduled()
    {
        var interview = new Interview { Status = InterviewStatus.Scheduled };

        Assert.Equal("scheduled", interview.StatusAsString);
    }

    [Fact]
    public void StatusAsString_WhenPassed_ReturnsPassed()
    {
        var interview = new Interview { Status = InterviewStatus.Passed };

        Assert.Equal("passed", interview.StatusAsString);
    }

    [Fact]
    public void StatusAsString_WhenApproved_ReturnsApproved()
    {
        var interview = new Interview { Status = InterviewStatus.Approved };

        Assert.Equal("approved", interview.StatusAsString);
    }

    [Fact]
    public void StatusAsString_WhenRejected_ReturnsRejected()
    {
        var interview = new Interview { Status = InterviewStatus.Rejected };

        Assert.Equal("rejected", interview.StatusAsString);
    }

    [Fact]
    public void StatusAsString_WhenToNextStage_ReturnsToNextStage()
    {
        var interview = new Interview { Status = InterviewStatus.ToNextStage };

        Assert.Equal("to_next_stage", interview.StatusAsString);
    }

    [Fact]
    public void StatusAsString_WhenInvalidValue_ReturnsUnknown()
    {
        var interview = new Interview { Status = (InterviewStatus)999 };

        Assert.Equal("unknown", interview.StatusAsString);
    }
}
