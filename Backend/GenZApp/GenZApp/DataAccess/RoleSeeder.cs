using Microsoft.AspNetCore.Identity;

namespace SchoolManagement.Infrastructure.Data;

public static class RoleSeeder
{
    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Admin"};

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(
                    new IdentityRole(role));

                if (!result.Succeeded)
                    throw new InvalidOperationException(
                        $"Unable to create role: {role}");
            }
        }
    }
}