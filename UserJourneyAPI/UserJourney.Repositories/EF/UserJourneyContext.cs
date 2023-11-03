namespace UserJourney.Repositories.EF
{
    using Microsoft.EntityFrameworkCore;

    public partial class UserJourneyContext : DbContext
    {
        public UserJourneyContext()
        {
        }

        public UserJourneyContext(DbContextOptions<UserJourneyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
    }
}
