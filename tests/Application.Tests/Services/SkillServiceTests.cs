using Application.Dtos;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Tests.Services;

public class SkillServiceTests
{
    private readonly Mock<ISkillRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly SkillService _service;

    public SkillServiceTests()
    {
        _repositoryMock = new Mock<ISkillRepository>();
        _mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<ILogger<SkillService>>();
        _service = new SkillService(_repositoryMock.Object, _mapperMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedSkills()
    {
        var skills = new List<Skill> { new() { Id = 1, Name = "C#" } };
        var expectedDtos = new List<SkillDto> { new() { Id = 1, Name = "C#" } };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(skills);
        _mapperMock.Setup(m => m.Map<IReadOnlyList<SkillDto>>(skills)).Returns(expectedDtos);

        var result = await _service.GetAllAsync();

        Assert.Equal(expectedDtos, result);
    }

    [Fact]
    public async Task CreateAsync_MapsAndAdds_ReturnsMappedDto()
    {
        var request = new CreateSkillRequest { Name = "C#" };
        var skill = new Skill { Name = "C#" };
        var expectedDto = new SkillDto { Id = 1, Name = "C#" };
        _mapperMock.Setup(m => m.Map<Skill>(request)).Returns(skill);
        _mapperMock.Setup(m => m.Map<SkillDto>(skill)).Returns(expectedDto);

        var result = await _service.CreateAsync(request);

        _repositoryMock.Verify(r => r.AddAsync(skill), Times.Once);
        Assert.Equal(expectedDto, result);
    }

    [Fact]
    public async Task UpdateAsync_WhenExists_MapsAndUpdates()
    {
        var id = 1;
        var request = new UpdateSkillRequest { Name = "C# Advanced" };
        var existingSkill = new Skill { Id = id, Name = "C#" };
        var expectedDto = new SkillDto { Id = id, Name = "C# Advanced" };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingSkill);
        _mapperMock.Setup(m => m.Map<SkillDto>(existingSkill)).Returns(expectedDto);

        var result = await _service.UpdateAsync(id, request);

        _mapperMock.Verify(m => m.Map(request, existingSkill), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(existingSkill), Times.Once);
        Assert.Equal(expectedDto, result);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotExists_ThrowsException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Skill?)null);

        var exception = await Assert.ThrowsAsync<Exception>(() => _service.UpdateAsync(1, new UpdateSkillRequest()));

        Assert.Contains("1", exception.Message);
    }
}
