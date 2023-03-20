using IssuesGalore.Models.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace WpfApp.Context;

internal class DataContext : DbContext
{
    public DataContext() { }

    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // TODO: Update this string to your local project path
        string projectDirectory = @"D:\Dev\yh\datalagring\IssuesGalore\IssuesGalore";

        SqlConnectionStringBuilder builder =
            new()
            {
                ["Data Source"] = @"(LocalDB)\MSSQLLocalDB",
                ["AttachDbFilename"] = @$"{projectDirectory}\Context\issues_galore_db.mdf",
                ["integrated Security"] = true,
                ["Connect Timeout"] = 30
            };

        optionsBuilder.UseSqlServer(builder.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<TicketEntity> Tickets { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }
    public DbSet<CustomerEntity> Customers { get; set; }
    public DbSet<StatusEntity> Statuses { get; set; }
}
