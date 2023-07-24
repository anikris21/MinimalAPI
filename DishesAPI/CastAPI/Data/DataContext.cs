using Microsoft.EntityFrameworkCore;

namespace CastAPI.Data
{
    public class DataContext :DbContext
    {
        public DataContext(DbContextOptions<DataContext> dbContextOptions) : base(dbContextOptions) { 
        
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Character>? Characters { get; set; }
    }
}
