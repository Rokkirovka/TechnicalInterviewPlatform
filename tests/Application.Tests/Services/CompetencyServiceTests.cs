using Application.Dtos;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Tests.Services;

public class CompetencyServiceTests
{
    private readonly Mock<ICompetencyRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CompetencyService _service;

    public CompetencyServiceTests()
    {
        _repositoryMock = new Mock<ICompetencyRepository>();
        _mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<ILogger<CompetencyService>>();
        _service = new CompetencyService(_repositoryMock.Object, _mapperMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedCompetencies()
    {
        var competencies = new List<Competency> { new() { Id = 1, Name = "C#" } };
        var expectedDtos = new List<CompetencyDto> { new() { Id = 1, Name = "C#" } };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(competencies);
        _mapperMock.Setup(m => m.Map<IReadOnlyList<CompetencyDto>>(competencies)).Returns(expectedDtos);

        var result = await _service.GetAllAsync();

        Assert.Equal(expectedDtos, result);
    }

    [Fact]
    public async Task CreateAsync_MapsAndAdds_ReturnsMappedDto()
    {
        var request = new CreateCompetencyRequest { Name = "SQL" };
        var competency = new Competency { Name = "SQL" };
        var expectedDto = new CompetencyDto { Id = 1, Name = "SQL" };
        _mapperMock.Setup(m => m.Map<Competency>(request)).Returns(competency);
        _mapperMock.Setup(m => m.Map<CompetencyDto>(competency)).Returns(expectedDto);

        var result = await _service.CreateAsync(request);

        _repositoryMock.Verify(r => r.AddAsync(competency), Times.Once);
        Assert.Equal(expectedDto, result);
    }

    [Fact]
    public async Task UpdateAsync_WhenExists_MapsAndUpdates()
    {
        var id = 1;
        var request = new UpdateCompetencyRequest { Name = "Advanced SQL", IsActive = true };
        var existing = new Competency { Id = id, Name = "SQL" };
        var expectedDto = new CompetencyDto { Id = id, Name = "Advanced SQL" };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
        _mapperMock.Setup(m => m.Map<CompetencyDto>(existing)).Returns(expectedDto);

        var result = await _service.UpdateAsync(id, request);

        _mapperMock.Verify(m => m.Map(request, existing), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(existing), Times.Once);
        Assert.Equal(expectedDto, result);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotExists_ThrowsException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Competency?)null);

        var exception = await Assert.ThrowsAsync<Exception>(() => _service.UpdateAsync(1, new UpdateCompetencyRequest()));

        Assert.Contains("1", exception.Message);
    }
}
