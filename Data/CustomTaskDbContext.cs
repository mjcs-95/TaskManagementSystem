using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static CustomTaskCore;

namespace Data
{
    internal class CustomTaskDbContext : DbContext
    {
        public DbSet<CustomTask> Tasks { get; set; }

        public CustomTaskDbContext(DbContextOptions<CustomTaskDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomTask>(ConfigureCustomTask);
            base.OnModelCreating(modelBuilder);
        }

        private static void ConfigureCustomTask(EntityTypeBuilder<CustomTask> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Title).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Description).HasMaxLength(500);
            builder.Property(t => t.DueDate).IsRequired();
        }
    }
}
