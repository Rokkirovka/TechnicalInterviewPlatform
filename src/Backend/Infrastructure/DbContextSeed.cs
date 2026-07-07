using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data;

public static class DbContextSeed
{
    public static async Task SeedAsync(
        ApplicationDbContext context, 
        IPasswordHasher passwordHasher, 
        IConfiguration configuration
        )
    {
        if (await context.Roles.AnyAsync())
        {
            return;
        }

        var adminRole = new Role
        {
            Name = "Administrator", 
            Description = "Полный доступ к системе"
        };
        var hrRole = new Role
        {
            Name = "HumanResources", 
            Description = "Управление кандидатами и Проведение интервью"
        };
        var solverRole = new Role
        {
            Name = "DecisionMaker", 
            Description = "Принятие итогово решения на основе матрицы компетенций и комментариев от HR"
        };

        context.Roles.AddRange(adminRole, hrRole, solverRole);
        await context.SaveChangesAsync();

        var adminUser = new User
        {
            Login = "admin",
            FullName = "Администратор",
            IsActive = true
        };
        
        var adminPassword = configuration["Admin:DefaultPassword"] 
                            ?? throw new InvalidOperationException("Admin password is required");
        adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, adminPassword);
        adminUser.Roles.Add(adminRole);

        context.Users.Add(adminUser);
        await context.SaveChangesAsync();
    }
}