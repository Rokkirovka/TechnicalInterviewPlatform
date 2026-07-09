using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure;

public static class DbContextSeed
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IConfiguration configuration)
    {
        var adminRole = await EnsureRoleAsync(
            context,
            name: "Administrator",
            description: "Full access to the system");

        var hrRole = await EnsureRoleAsync(
            context,
            name: "HumanResources",
            description: "Candidate management and interviews");

        await EnsureRoleAsync(
            context,
            name: "DecisionMaker",
            description: "Final hiring decision maker");

        var adminPassword = configuration["Admin:DefaultPassword"]
                            ?? throw new InvalidOperationException("Admin password is required");
        var hrPassword = configuration["Hr:DefaultPassword"] ?? adminPassword;

        await EnsureUserAsync(
            context,
            passwordHasher,
            login: "admin",
            fullName: "Administrator",
            password: adminPassword,
            role: adminRole);

        await EnsureUserAsync(
            context,
            passwordHasher,
            login: "hr",
            fullName: "Human Resources",
            password: hrPassword,
            role: hrRole);
    }

    private static async Task<Role> EnsureRoleAsync(
        ApplicationDbContext context,
        string name,
        string description)
    {
        var existingRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == name);
        if (existingRole != null)
        {
            return existingRole;
        }

        var role = new Role
        {
            Name = name,
            Description = description
        };

        context.Roles.Add(role);
        await context.SaveChangesAsync();
        return role;
    }

    private static async Task EnsureUserAsync(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        string login,
        string fullName,
        string password,
        Role role)
    {
        var existingUser = await context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Login == login);

        if (existingUser != null)
        {
            existingUser.FullName = fullName;
            existingUser.IsActive = true;
            existingUser.PasswordHash = passwordHasher.HashPassword(existingUser, password);

            if (existingUser.Roles.All(r => r.Name != role.Name))
            {
                existingUser.Roles.Add(role);
            }

            await context.SaveChangesAsync();
            return;
        }

        var user = new User
        {
            Login = login,
            FullName = fullName,
            IsActive = true
        };

        user.PasswordHash = passwordHasher.HashPassword(user, password);
        user.Roles.Add(role);

        context.Users.Add(user);
        await context.SaveChangesAsync();
    }
}
