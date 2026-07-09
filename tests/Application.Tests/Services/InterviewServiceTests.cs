using Application.Dtos;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Tests.Services;

public class InterviewServiceTests
{
    private readonly Mock<IInterviewRepository> _interviewRepoMock;
    private readonly Mock<IVacancyRepository> _vacancyRepoMock;
    private readonly Mock<ICandidateRepository> _candidateRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IDeletionLogRepository<Interview>> _deletionLogRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly InterviewService _service;

    public InterviewServiceTests()
    {
        _interviewRepoMock = new Mock<IInterviewRepository>();
        _vacancyRepoMock = new Mock<IVacancyRepository>();
        _candidateRepoMock = new Mock<ICandidateRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _deletionLogRepoMock = new Mock<IDeletionLogRepository<Interview>>();
        _mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<ILogger<InterviewService>>();
        _service = new InterviewService(
            _interviewRepoMock.Object,
            _vacancyRepoMock.Object,
            _candidateRepoMock.Object,
            _userRepoMock.Object,
            _deletionLogRepoMock.Object,
            _mapperMock.Object,
            loggerMock.Object);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMappedInterviews()
    {
        var interviews = new List<Interview> { new() { Id = 1 } };
        var expectedDtos = new List<InterviewDto> { new() { Id = 1 } };
        _interviewRepoMock.Setup(r => r.SearchAsync("test", false)).ReturnsAsync(interviews);
        _mapperMock.Setup(m => m.Map<IReadOnlyList<InterviewDto>>(interviews)).Returns(expectedDtos);

        var result = await _service.SearchAsync("test", false);

        Assert.Equal(expectedDtos, result);
    }

    [Fact]
    public async Task GetByCandidateIdAsync_ReturnsMappedInterviews()
    {
        var interviews = new List<Interview> { new() { Id = 1, CandidateId = 5 } };
        var expectedDtos = new List<InterviewDto> { new() { Id = 1 } };
        _interviewRepoMock.Setup(r => r.GetByCandidateIdAsync(5)).ReturnsAsync(interviews);
        _mapperMock.Setup(m => m.Map<IReadOnlyList<InterviewDto>>(interviews)).Returns(expectedDtos);

        var result = await _service.GetByCandidateIdAsync(5);

        Assert.Equal(expectedDtos, result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsMappedDto()
    {
        var interview = new Interview { Id = 1 };
        var expectedDto = new InterviewDto { Id = 1 };
        _interviewRepoMock.Setup(r => r.GetWithDetailsAsync(1)).ReturnsAsync(interview);
        _mapperMock.Setup(m => m.Map<InterviewDto>(interview)).Returns(expectedDto);

        var result = await _service.GetByIdAsync(1);

        Assert.Equal(expectedDto, result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _interviewRepoMock.Setup(r => r.GetWithDetailsAsync(1)).ReturnsAsync((Interview?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(1));
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesInterviewWithStagesAndScores()
    {
        var request = new CreateInterviewRequest
        {
            CandidateId = 10,
            VacancyId = 20,
            ScheduledAt = new DateTime(2025, 6, 1, 10, 0, 0, DateTimeKind.Utc),
            Stages =
            [
                new CreateInterviewStageRequest { Name = "HR", Duration = 30, StageNumber = 1 }
            ]
        };
        var createdByUserId = 5;

        var vacancy = new Vacancy { Id = 20 };
        vacancy.VacancyCompetencies.Add(new VacancyCompetency { CompetencyId = 100 });
        vacancy.VacancyCompetencies.Add(new VacancyCompetency { CompetencyId = 200 });
        var candidate = new Candidate { Id = 10 };
        var creator = new User { Id = 5 };

        _vacancyRepoMock.Setup(r => r.GetWithCompetenciesAsync(20)).ReturnsAsync(vacancy);
        _candidateRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(candidate);
        _userRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(creator);
        _mapperMock.Setup(m => m.Map<InterviewDto>(It.IsAny<Interview>())).Returns(new InterviewDto());

        await _service.CreateAsync(request, createdByUserId);

        _interviewRepoMock.Verify(r => r.AddAsync(It.Is<Interview>(i =>
            i.CandidateId == 10 &&
            i.VacancyId == 20 &&
            i.Status == InterviewStatus.Scheduled &&
            i.CreatedByUserId == 5 &&
            i.InterviewStages.Count == 1 &&
            i.InterviewStages.First().StageName == "HR" &&
            i.CompetencyScores.Count == 2 &&
            i.CompetencyScores.All(cs => cs.Score == 0)
        )), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenVacancyNotFound_ThrowsKeyNotFoundException()
    {
        _vacancyRepoMock.Setup(r => r.GetWithCompetenciesAsync(1)).ReturnsAsync((Vacancy?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.CreateAsync(new CreateInterviewRequest { VacancyId = 1 }, 1));
    }

    [Fact]
    public async Task CreateAsync_WhenCandidateNotFound_ThrowsKeyNotFoundException()
    {
        var vacancy = new Vacancy { Id = 1 };
        _vacancyRepoMock.Setup(r => r.GetWithCompetenciesAsync(1)).ReturnsAsync(vacancy);
        _candidateRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Candidate?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.CreateAsync(new CreateInterviewRequest { VacancyId = 1, CandidateId = 99 }, 1));
    }

    [Fact]
    public async Task UpdateAsync_WhenExists_UpdatesScoresAndAddsComment()
    {
        var id = 1;
        var request = new UpdateInterviewRequest
        {
            ScheduledAt = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc),
            Matrix = [new UpdateCompetencyScoreRequest { Id = 50, Score = 5 }],
            Comment = "Good candidate"
        };
        var interview = new Interview
        {
            Id = id,
            ScheduledAt = default,
            CompetencyScores = [new CompetencyScore { Id = 50, Score = 0 }],
            Comments = []
        };
        _interviewRepoMock.Setup(r => r.GetWithDetailsAsync(id)).ReturnsAsync(interview);
        _mapperMock.Setup(m => m.Map<InterviewDto>(interview)).Returns(new InterviewDto());

        await _service.UpdateAsync(id, request, 42);

        Assert.Equal(request.ScheduledAt, interview.ScheduledAt);
        Assert.Equal(5, interview.CompetencyScores.First().Score);
        Assert.Single(interview.Comments);
        Assert.Equal("Good candidate", interview.Comments.First().Content);
        Assert.Equal(42, interview.Comments.First().AuthorId);
        _interviewRepoMock.Verify(r => r.UpdateAsync(interview), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _interviewRepoMock.Setup(r => r.GetWithDetailsAsync(1)).ReturnsAsync((Interview?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.UpdateAsync(1, new UpdateInterviewRequest(), 1));
    }

    [Fact]
    public async Task MarkPassedAsync_SetsStatusToPassed()
    {
        var interview = new Interview { Id = 1, Status = InterviewStatus.Scheduled };
        _interviewRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(interview);

        await _service.MarkPassedAsync(1);

        Assert.Equal(InterviewStatus.Passed, interview.Status);
        _interviewRepoMock.Verify(r => r.UpdateAsync(interview), Times.Once);
    }

    [Fact]
    public async Task MarkPassedAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _interviewRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Interview?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.MarkPassedAsync(1));
    }

    [Theory]
    [InlineData("approved", InterviewStatus.Approved)]
    [InlineData("rejected", InterviewStatus.Rejected)]
    [InlineData("to_next_stage", InterviewStatus.ToNextStage)]
    public async Task SetDecisionAsync_WithValidDecision_SetsStatus(string decision, InterviewStatus expectedStatus)
    {
        var interview = new Interview { Id = 1 };
        _interviewRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(interview);

        await _service.SetDecisionAsync(1, decision);

        Assert.Equal(expectedStatus, interview.Status);
        _interviewRepoMock.Verify(r => r.UpdateAsync(interview), Times.Once);
    }

    [Fact]
    public async Task SetDecisionAsync_WithInvalidDecision_ThrowsArgumentException()
    {
        var interview = new Interview { Id = 1 };
        _interviewRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(interview);

        await Assert.ThrowsAsync<ArgumentException>(() => _service.SetDecisionAsync(1, "invalid"));
    }

    [Fact]
    public async Task SetDecisionAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _interviewRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Interview?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.SetDecisionAsync(1, "approved"));
    }

    [Fact]
    public async Task ArchiveAsync_SetsDeletedAtAndLogs()
    {
        var interview = new Interview { Id = 1 };
        _interviewRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(interview);

        await _service.ArchiveAsync(1, "reason", 42);

        Assert.NotNull(interview.DeletedAt);
        _interviewRepoMock.Verify(r => r.UpdateAsync(interview), Times.Once);
        _deletionLogRepoMock.Verify(r => r.AddAsync(interview, 42, "reason"), Times.Once);
    }

    [Fact]
    public async Task ArchiveAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _interviewRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Interview?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ArchiveAsync(1, "reason", 42));
    }

    [Fact]
    public async Task RestoreAsync_ClearsDeletedAt()
    {
        var interview = new Interview { Id = 1, DeletedAt = DateTime.UtcNow };
        _interviewRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(interview);

        await _service.RestoreAsync(1);

        Assert.Null(interview.DeletedAt);
        _interviewRepoMock.Verify(r => r.UpdateAsync(interview), Times.Once);
    }

    [Fact]
    public async Task RestoreAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _interviewRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Interview?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.RestoreAsync(1));
    }
}
