using Domain.Entities;
using Domain.Enums;

namespace Domain.Tests.EntityTests;

public class CandidateTests
{
    [Fact]
    public void FullName_WithAllParts_ReturnsConcatenatedString()
    {
        var candidate = new Candidate { FirstName = "Иван", LastName = "Иванов", MiddleName = "Иванович" };

        var fullName = candidate.FullName;

        Assert.Equal("Иван Иванов Иванович", fullName);
    }

    [Fact]
    public void FullName_WithEmptyMiddleName_IncludesEmptyMiddle()
    {
        var candidate = new Candidate { FirstName = "John", LastName = "Doe", MiddleName = "" };

        var fullName = candidate.FullName;

        Assert.Equal("John Doe ", fullName);
    }

    [Fact]
    public void Status_WithNoInterviews_ReturnsNew()
    {
        var candidate = new Candidate { Interviews = [] };

        var status = candidate.Status;

        Assert.Equal("new", status);
    }

    [Fact]
    public void Status_WithApprovedInterview_ReturnsAccepted()
    {
        var candidate = new Candidate
        {
            Interviews =
            [
                new Interview { Status = InterviewStatus.Scheduled },
                new Interview { Status = InterviewStatus.Approved }
            ]
        };

        var status = candidate.Status;

        Assert.Equal("accepted", status);
    }

    [Fact]
    public void Status_WithOnlyNonApprovedInterviews_ReturnsInProgress()
    {
        var candidate = new Candidate
        {
            Interviews =
            [
                new Interview { Status = InterviewStatus.Scheduled },
                new Interview { Status = InterviewStatus.Passed }
            ]
        };

        var status = candidate.Status;

        Assert.Equal("in_progress", status);
    }

    [Fact]
    public void Status_WithRejectedInterview_ReturnsInProgress()
    {
        var candidate = new Candidate
        {
            Interviews =
            [
                new Interview { Status = InterviewStatus.Rejected }
            ]
        };

        var status = candidate.Status;

        Assert.Equal("in_progress", status);
    }
}
