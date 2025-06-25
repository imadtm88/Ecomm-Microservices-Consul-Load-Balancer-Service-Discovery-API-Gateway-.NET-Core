using Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Data
{
    public class ClientDbContext : DbContext
    {
        public ClientDbContext(DbContextOptions<ClientDbContext> options) : base(options) { }

        public DbSet<Client> Clients => Set<Client>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.FirstName).IsRequired();
                entity.Property(c => c.LastName).IsRequired();
                entity.Property(c => c.Email).IsRequired();
                entity.Property(c => c.Phone).IsRequired();
                entity.Property(c => c.Address).IsRequired();
                entity.Property(c => c.CreatedAt).IsRequired();
            });
        }
    }
}
