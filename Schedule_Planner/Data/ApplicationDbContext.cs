using Microsoft.EntityFrameworkCore;
using Schedule_Planner.Models;

namespace Schedule_Planner.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
    {
        
    }
    public DbSet<UserModel> User { get; set; }
    public DbSet<SubjectModel> Subject { get; set; }
    public DbSet<ScheduleModel> Schedule { get; set; }
}