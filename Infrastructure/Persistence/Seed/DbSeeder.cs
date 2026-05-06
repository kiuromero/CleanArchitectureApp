using Microsoft.EntityFrameworkCore;

public static class DbSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "$2a$11$k6q6EpqwxiY9Ol1XRlwvP.cGHoODOzQlYRrw3wTjy2hG5uGgaOQTe",
                Role = "Admin"
            }
        );
    }
}