using MvcIdentity.Models;
using System.Data.Entity;

namespace MvcIdentity.Models
{
    public class MvcIdentityContext : DbContext
    {
        public MvcIdentityContext() : base("MvcIdentityConnection") { }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Member> Members { get; set; }
        public static MvcIdentityContext Create()
        {
            return new MvcIdentityContext();
        }
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //  modelBuilder.Entity<Article>()
        //    .ToTable("Contents")
        //    .HasKey(a => a.Url)
        //    .Property(a => a.Url)
        //    .HasColumnName("Address")
        //    .HasMaxLength(200);

        //}
    }
}