using Microsoft.EntityFrameworkCore;

namespace RxCats.GameData
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(string connectionString) : base(CreateConnection(connectionString))
        {

        }

        private static DbContextOptions CreateConnection(string connectionString)
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseMySql(connectionString);
            return builder.Options;
        }

        public DbSet<Person> Person { get; set; }
    }
}
