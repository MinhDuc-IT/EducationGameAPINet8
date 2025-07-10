using EducationGameAPINet8.Entities;
using Microsoft.EntityFrameworkCore;

namespace EducationGameAPINet8.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<GameSession> GameSessions { get; set; }
    }
}
