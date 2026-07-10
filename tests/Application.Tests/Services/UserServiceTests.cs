using Application.Dtos;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IRoleRepository> _roleRepoMock;
    private readonly Mock<IDeletionLogRepository<User>> _deletionLogRepoMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _roleRepoMock = new Mock<IRoleRepository>();
        _deletionLogRepoMock = new Mock<IDeletionLogRepository<User>>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<ILogger<UserService>>();
        _service = new UserService(
            _userRepoMock.Object,
            _roleRepoMock.Object,
            _deletionLogRepoMock.Object,
            _passwordHasherMock.Object,
            _mapperMock.Object,
            loggerMock.Object);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMappedUsers()
    {
        var users = new List<User> { new() { Id = 1 } };
        var expectedDtos = new List<UserDto> { new() { Id = 1 } };
        _userRepoMock.Setup(r => r.SearchAsync("test", false)).ReturnsAsync(users);
        _mapperMock.Setup(m => m.Map<IReadOnlyList<UserDto>>(users)).Returns(expectedDtos);

        var result = await _service.SearchAsync("test", false);

        Assert.Equal(expectedDtos, result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsMappedDto()
    {
        var user = new User { Id = 1 };
        var expectedDto = new UserDto { Id = 1 };
        _userRepoMock.Setup(r => r.GetWithRolesAsync(1)).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(expectedDto);

        var result = await _service.GetByIdAsync(1);

        Assert.Equal(expectedDto, result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _userRepoMock.Setup(r => r.GetWithRolesAsync(1)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(1));
    }

    [Fact]
    public async Task CreateAsync_HashesPasswordAndAssignsRoles()
    {
        var request = new CreateUserRequest
        {
            Login = "test",
            Password = "secret",
            Roles = ["admin", "hr"]
        };
        var user = new User();
        var allRoles = new List<Role>
        {
            new() { Id = 1, Name = "admin" },
            new() { Id = 2, Name = "hr" },
            new() { Id = 3, Name = "approver" }
        };
        _mapperMock.Setup(m => m.Map<User>(request)).Returns(user);
        _passwordHasherMock.Setup(p => p.HashPassword(user, "secret")).Returns("hashed");
        _roleRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(allRoles);
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto());

        await _service.CreateAsync(request);

        Assert.Equal("hashed", user.PasswordHash);
        Assert.True(user.IsActive);
        Assert.Equal(2, user.Roles.Count);
        Assert.Contains(user.Roles, r => r.Name == "admin");
        Assert.Contains(user.Roles, r => r.Name == "hr");
        _userRepoMock.Verify(r => r.AddAsync(user), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNoRoles_AssignsEmptyRoles()
    {
        var request = new CreateUserRequest { Login = "test", Password = "secret", Roles = [] };
        var user = new User();
        _mapperMock.Setup(m => m.Map<User>(request)).Returns(user);
        _passwordHasherMock.Setup(p => p.HashPassword(user, "secret")).Returns("hashed");
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto());

        await _service.CreateAsync(request);

        Assert.Empty(user.Roles);
    }

    [Fact]
    public async Task UpdateAsync_WhenExists_UpdatesFields()
    {
        var id = 1;
        var request = new UpdateUserRequest
        {
            Login = "updated",
            Password = "newpass",
            Roles = ["admin"]
        };
        var user = new User { Id = id };
        var allRoles = new List<Role> { new() { Id = 1, Name = "admin" } };
        _userRepoMock.Setup(r => r.GetWithRolesAsync(id)).ReturnsAsync(user);
        _passwordHasherMock.Setup(p => p.HashPassword(user, "newpass")).Returns("newhash");
        _roleRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(allRoles);
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto());

        await _service.UpdateAsync(id, request);

        _mapperMock.Verify(m => m.Map(request, user), Times.Once);
        Assert.Equal("newhash", user.PasswordHash);
        Assert.Single(user.Roles);
        _userRepoMock.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenPasswordEmpty_DoesNotRehash()
    {
        var id = 1;
        var request = new UpdateUserRequest { Password = "", Roles = [] };
        var user = new User { Id = id, PasswordHash = "oldhash" };
        _userRepoMock.Setup(r => r.GetWithRolesAsync(id)).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto());

        await _service.UpdateAsync(id, request);

        _passwordHasherMock.Verify(p => p.HashPassword(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        Assert.Equal("oldhash", user.PasswordHash);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _userRepoMock.Setup(r => r.GetWithRolesAsync(1)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(1, new UpdateUserRequest()));
    }

    [Fact]
    public async Task ArchiveAsync_WhenSelfArchiving_ThrowsBusinessRuleConflictException()
    {
        var user = new User { Id = 1 };
        _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        var exception = await Assert.ThrowsAsync<BusinessRuleConflictException>(
            () => _service.ArchiveAsync(1, "reason", 1));

        Assert.Contains("самого себя", exception.Message.ToLower());
    }

    [Fact]
    public async Task ArchiveAsync_WhenArchivingOther_ArchivesSuccessfully()
    {
        var user = new User { Id = 1 };
        _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        await _service.ArchiveAsync(1, "reason", 2);

        Assert.NotNull(user.DeletedAt);
        _userRepoMock.Verify(r => r.UpdateAsync(user), Times.Once);
        _deletionLogRepoMock.Verify(r => r.AddAsync(user, 2, "reason"), Times.Once);
    }

    [Fact]
    public async Task ArchiveAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ArchiveAsync(1, "reason", 2));
    }

    [Fact]
    public async Task RestoreAsync_ClearsDeletedAt()
    {
        var user = new User { Id = 1, DeletedAt = DateTime.UtcNow };
        _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        await _service.RestoreAsync(1);

        Assert.Null(user.DeletedAt);
        _userRepoMock.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task RestoreAsync_WhenNotExists_ThrowsKeyNotFoundException()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.RestoreAsync(1));
    }
}
