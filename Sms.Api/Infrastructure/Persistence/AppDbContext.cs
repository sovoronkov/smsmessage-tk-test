
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Configuration;


namespace Persistence.Main;

public class AppDbContext : DbContext
{

    public DbSet<ClientMessageHeader> ClientMessageHeaders { get; set; }
    public DbSet<ClientMessageBody> ClientMessageBodies { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ClientMessageHeaderConfiguration());
        modelBuilder.ApplyConfiguration(new ClientMessageBodyConfiguration());
    }
}