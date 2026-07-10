using Application.Dtos;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Tests.Services;

public class VacancyServiceTests
{
    private readonly Mock<IVacancyRepository> _vacancyRepoMock;
    private readonly Mock<ICompetencyRepository> _competencyRepoMock;
    private readonly Mock<IDeletionLogRepository<Vacancy>> _deletionLogRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly VacancyService _service;

    public VacancyServiceTests()
    {
        _vacancyRepoMock = new Mock<IVacancyRepository>();
        _competencyRepoMock = new Mock<ICompetencyRepository>();
        _deletionLogRepoMock = new Mock<IDeletionLogRepository<Vacancy>>();
        _mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<ILogger<VacancyService>>();
        _service = new VacancyService(
            _vacancyRepoMock.Object,
            _competencyRepoMock.Object,
            _deletionLogRepoMock.Object,
            _mapperMock.Object,
            loggerMock.Object);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMappedVacancies()
    {
        var vacancies = new List<Vacancy> { new() { Id = 1 } };
        var expectedDtos = new List<VacancyDto> { new() { Id = 1 } };
        _vacancyRepoMock.Setup(r => r.SearchAsync("test", false)).ReturnsAsync(vacancies);
        _mapperMock.Setup(m => m.Map<IReadOnlyList<VacancyDto>>(vacancies)).Returns(expectedDtos);

        var result = await _service.SearchAsync("test", false);

        Assert.Equal(expectedDtos, result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsMappedDto()
    {
        var vacancy = new Vacancy { Id = 1 };
        var expectedDto = new VacancyDto { Id = 1 };
        _vacancyRepoMock.Setup(r => r.GetWithCompetenciesAsync(1)).ReturnsAsync(vacancy);
        _mapperMock.Setup(m => m.Map<VacancyDto>(vacancy)).Returns(expectedDto);

        var result = await _service.GetByIdAsync(1);

        Assert.Equal(expectedDto, result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _vacancyRepoMock.Setup(r => r.GetWithCompetenciesAsync(1)).ReturnsAsync((Vacancy?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(1));
    }

    [Fact]
    public async Task CreateAsync_WithCompetencyIds_AssignsCompetencies()
    {
        var request = new CreateVacancyRequest { Title = "Developer", CompetencyIds = [1, 2] };
        var vacancy = new Vacancy { Id = 1 };
        var competencies = new List<Competency>
        {
            new() { Id = 1, Name = "C#" },
            new() { Id = 2, Name = "SQL" }
        };
        _mapperMock.Setup(m => m.Map<Vacancy>(request)).Returns(vacancy);
        _competencyRepoMock.Setup(r => r.GetByIdsAsync(new List<int> { 1, 2 })).ReturnsAsync(competencies);
        _mapperMock.Setup(m => m.Map<VacancyDto>(vacancy)).Returns(new VacancyDto());

        await _service.CreateAsync(request);

        Assert.Equal(2, vacancy.VacancyCompetencies.Count);
        Assert.Contains(vacancy.VacancyCompetencies, vc => vc.CompetencyId == 1);
        Assert.Contains(vacancy.VacancyCompetencies, vc => vc.CompetencyId == 2);
        _vacancyRepoMock.Verify(r => r.AddAsync(vacancy), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyCompetencyIds_CreatesWithoutCompetencies()
    {
        var request = new CreateVacancyRequest { Title = "Developer", CompetencyIds = [] };
        var vacancy = new Vacancy { Id = 1 };
        _mapperMock.Setup(m => m.Map<Vacancy>(request)).Returns(vacancy);
        _mapperMock.Setup(m => m.Map<VacancyDto>(vacancy)).Returns(new VacancyDto());

        await _service.CreateAsync(request);

        Assert.Empty(vacancy.VacancyCompetencies);
        _competencyRepoMock.Verify(r => r.GetByIdsAsync(It.IsAny<List<int>>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenExists_ClearsAndReassignsCompetencies()
    {
        var id = 1;
        var request = new UpdateVacancyRequest { Title = "Senior Dev", CompetencyIds = [3] };
        var vacancy = new Vacancy { Id = id };
        vacancy.VacancyCompetencies.Add(new VacancyCompetency { CompetencyId = 99 });
        var competencies = new List<Competency> { new() { Id = 3, Name = "Azure" } };
        _vacancyRepoMock.Setup(r => r.GetWithCompetenciesAsync(id)).ReturnsAsync(vacancy);
        _competencyRepoMock.Setup(r => r.GetByIdsAsync(new List<int> { 3 })).ReturnsAsync(competencies);
        _mapperMock.Setup(m => m.Map<VacancyDto>(vacancy)).Returns(new VacancyDto());

        await _service.UpdateAsync(id, request);

        Assert.Single(vacancy.VacancyCompetencies);
        Assert.Equal(3, vacancy.VacancyCompetencies.First().CompetencyId);
        _vacancyRepoMock.Verify(r => r.UpdateAsync(vacancy), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _vacancyRepoMock.Setup(r => r.GetWithCompetenciesAsync(1)).ReturnsAsync((Vacancy?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(1, new UpdateVacancyRequest()));
    }

    [Fact]
    public async Task ArchiveAsync_SetsDeletedAtAndLogs()
    {
        var vacancy = new Vacancy { Id = 1 };
        _vacancyRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(vacancy);

        await _service.ArchiveAsync(1, "reason", 42);

        Assert.NotNull(vacancy.DeletedAt);
        _vacancyRepoMock.Verify(r => r.UpdateAsync(vacancy), Times.Once);
        _deletionLogRepoMock.Verify(r => r.AddAsync(vacancy, 42, "reason"), Times.Once);
    }

    [Fact]
    public async Task ArchiveAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _vacancyRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Vacancy?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ArchiveAsync(1, "reason", 42));
    }

    [Fact]
    public async Task RestoreAsync_ClearsDeletedAt()
    {
        var vacancy = new Vacancy { Id = 1, DeletedAt = DateTime.UtcNow };
        _vacancyRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(vacancy);

        await _service.RestoreAsync(1);

        Assert.Null(vacancy.DeletedAt);
        _vacancyRepoMock.Verify(r => r.UpdateAsync(vacancy), Times.Once);
    }

    [Fact]
    public async Task RestoreAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _vacancyRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Vacancy?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.RestoreAsync(1));
    }
}
