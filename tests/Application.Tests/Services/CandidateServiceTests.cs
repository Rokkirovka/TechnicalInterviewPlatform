using Application.Dtos;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Tests.Services;

public class CandidateServiceTests
{
    private readonly Mock<ICandidateRepository> _candidateRepoMock;
    private readonly Mock<ISkillRepository> _skillRepoMock;
    private readonly Mock<IDeletionLogRepository<Candidate>> _deletionLogRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CandidateService _service;

    public CandidateServiceTests()
    {
        _candidateRepoMock = new Mock<ICandidateRepository>();
        _skillRepoMock = new Mock<ISkillRepository>();
        _deletionLogRepoMock = new Mock<IDeletionLogRepository<Candidate>>();
        _mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<ILogger<CandidateService>>();
        _service = new CandidateService(
            _candidateRepoMock.Object,
            _skillRepoMock.Object,
            _deletionLogRepoMock.Object,
            _mapperMock.Object,
            loggerMock.Object);
    }

    [Fact]
    public async Task SearchAsync_DelegatesToRepository()
    {
        var candidates = new List<Candidate> { new() { Id = 1 } };
        var expectedDtos = new List<CandidateDto> { new() { Id = 1 } };
        _candidateRepoMock.Setup(r => r.SearchAsync("test", false)).ReturnsAsync(candidates);
        _mapperMock.Setup(m => m.Map<IReadOnlyList<CandidateDto>>(candidates)).Returns(expectedDtos);

        var result = await _service.SearchAsync("test", false);

        Assert.Equal(expectedDtos, result);
    }

    [Fact]
    public async Task GetNamesAsync_DelegatesToRepository()
    {
        var candidates = new List<Candidate> { new() { Id = 1, FirstName = "A", LastName = "B" } };
        var expectedDtos = new List<CandidateNameDto> { new() { Id = 1, FullName = "A B " } };
        _candidateRepoMock.Setup(r => r.SearchAsync(null, false)).ReturnsAsync(candidates);
        _mapperMock.Setup(m => m.Map<IReadOnlyList<CandidateNameDto>>(candidates)).Returns(expectedDtos);

        var result = await _service.GetNamesAsync();

        Assert.Equal(expectedDtos, result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsMappedDto()
    {
        var candidate = new Candidate { Id = 1 };
        var expectedDto = new CandidateDto { Id = 1 };
        _candidateRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(candidate);
        _mapperMock.Setup(m => m.Map<CandidateDto>(candidate)).Returns(expectedDto);

        var result = await _service.GetByIdAsync(1);

        Assert.Equal(expectedDto, result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _candidateRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Candidate?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(1));
    }

    [Fact]
    public async Task CreateAsync_WithNoSkills_MapsAndAdds()
    {
        var request = new CreateCandidateRequest { Skills = [] };
        var candidate = new Candidate();
        var expectedDto = new CandidateDto { Id = 1 };
        _mapperMock.Setup(m => m.Map<Candidate>(request)).Returns(candidate);
        _mapperMock.Setup(m => m.Map<CandidateDto>(candidate)).Returns(expectedDto);

        var result = await _service.CreateAsync(request);

        _skillRepoMock.Verify(r => r.GetByNamesAsync(It.IsAny<List<string>>()), Times.Never);
        _candidateRepoMock.Verify(r => r.AddAsync(candidate), Times.Once);
        Assert.Equal(expectedDto, result);
    }

    [Fact]
    public async Task CreateAsync_WithSkills_MatchesByNameAndAdds()
    {
        var request = new CreateCandidateRequest
        {
            Skills =
            [
                new CreateCandidateSkillRequest { SkillName = "C#", Level = ProficiencyLevel.High },
                new CreateCandidateSkillRequest { SkillName = "UnknownSkill", Level = ProficiencyLevel.Beginner }
            ]
        };
        var candidate = new Candidate();
        var existingSkills = new List<Skill> { new() { Id = 10, Name = "C#" } };
        _mapperMock.Setup(m => m.Map<Candidate>(request)).Returns(candidate);
        _mapperMock.Setup(m => m.Map<CandidateDto>(candidate)).Returns(new CandidateDto());
        _skillRepoMock.Setup(r => r.GetByNamesAsync(It.IsAny<List<string>>())).ReturnsAsync(existingSkills);

        await _service.CreateAsync(request);

        _candidateRepoMock.Verify(r => r.AddAsync(candidate), Times.Once);
        Assert.Single(candidate.CandidateSkills);
        Assert.Equal(10, candidate.CandidateSkills.First().SkillId);
        Assert.Equal(ProficiencyLevel.High, candidate.CandidateSkills.First().Level);
    }

    [Fact]
    public async Task UpdateAsync_WhenExists_ClearsSkillsAndUpdates()
    {
        var id = 1;
        var request = new UpdateCandidateRequest { Skills = [] };
        var existingCandidate = new Candidate { Id = id };
        existingCandidate.CandidateSkills.Add(new CandidateSkill { SkillId = 99 });
        var expectedDto = new CandidateDto { Id = id };
        _candidateRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingCandidate);
        _mapperMock.Setup(m => m.Map<CandidateDto>(existingCandidate)).Returns(expectedDto);

        var result = await _service.UpdateAsync(id, request);

        Assert.Empty(existingCandidate.CandidateSkills);
        _mapperMock.Verify(m => m.Map(request, existingCandidate), Times.Once);
        _candidateRepoMock.Verify(r => r.UpdateAsync(existingCandidate), Times.Once);
        Assert.Equal(expectedDto, result);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _candidateRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Candidate?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(1, new UpdateCandidateRequest()));
    }

    [Fact]
    public async Task ArchiveAsync_SetsDeletedAtAndLogs()
    {
        var candidate = new Candidate { Id = 1 };
        _candidateRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(candidate);

        await _service.ArchiveAsync(1, "reason", 42);

        Assert.NotNull(candidate.DeletedAt);
        _candidateRepoMock.Verify(r => r.UpdateAsync(candidate), Times.Once);
        _deletionLogRepoMock.Verify(r => r.AddAsync(candidate, 42, "reason"), Times.Once);
    }

    [Fact]
    public async Task ArchiveAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _candidateRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Candidate?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ArchiveAsync(1, "reason", 42));
    }

    [Fact]
    public async Task RestoreAsync_ClearsDeletedAt()
    {
        var candidate = new Candidate { Id = 1, DeletedAt = DateTime.UtcNow };
        _candidateRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(candidate);

        await _service.RestoreAsync(1);

        Assert.Null(candidate.DeletedAt);
        _candidateRepoMock.Verify(r => r.UpdateAsync(candidate), Times.Once);
    }

    [Fact]
    public async Task RestoreAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _candidateRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Candidate?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.RestoreAsync(1));
    }
}
