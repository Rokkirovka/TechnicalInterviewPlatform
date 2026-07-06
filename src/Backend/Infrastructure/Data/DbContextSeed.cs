using Domain.Entities;
using Infrastructure.Auth.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public static class DbContextSeed
{
    public static async Task SeedAsync(ApplicationDbContext context, PasswordHasher passwordHasher)
    {
        if (await context.Roles.AnyAsync())
        {
            return;
        }

        var adminRole = new Role { Name = "Administrator", Description = "Полный доступ к системе" };
        var hrRole = new Role { Name = "HR", Description = "Управление кандидатами и интервью" };
        var solverRole = new Role { Name = "Решала", Description = "Проведение интервью и оценка компетенций" };

        context.Roles.AddRange(adminRole, hrRole, solverRole);
        await context.SaveChangesAsync();

        var adminUser = new User
        {
            Login = "admin",
            FullName = "Администратор",
            IsActive = true
        };
        adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "SuperB0t!");
        adminUser.Roles.Add(adminRole);

        context.Users.Add(adminUser);
        await context.SaveChangesAsync();
    }
}