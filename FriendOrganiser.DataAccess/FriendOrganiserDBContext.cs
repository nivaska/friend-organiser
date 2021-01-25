using FriendOrganiser.Model;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace FriendOrganiser.DataAccess
{
    public class FriendOrganiserDBContext: DbContext
    {
        public FriendOrganiserDBContext():base("FriendOrganizerDb")
        {

        }

        public DbSet<Friend> Friends { get; set; }
        public DbSet<ProgrammingLanguage> ProgrammingLanguages { get; set; }
        public DbSet<FriendPhoneNumber> FriendPhoneNumbers { get; set; }
        public DbSet<Meeting> Meetings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Fluent API to add constraints for DB fields (or use Attibutes in the Model using DataAnnotations)
            //modelBuilder.Configurations.Add(new FriendConfiguration());
        }
    }

    public class FriendConfiguration: EntityTypeConfiguration<Friend>
    {
        public FriendConfiguration()
        {
            Property(f => f.FirstName)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
