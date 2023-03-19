using IssuesGalore.Models.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace WpfApp.Context;

internal class DataContext : DbContext
{
    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName!;

        SqlConnectionStringBuilder builder = new()
        {
            ["Data Source"] = @"(LocalDB)\MSSQLLocalDB",
            ["AttachDbFilename"] = @$"{projectDirectory}\Context\issues_galore_db.mdf",
            ["integrated Security"] = true,
            ["Connect Timeout"] = 30
        };
        //Console.WriteLine(builder.ConnectionString);

        // Attempt to use a relative path to the db-file. Not sure if it works everywhere?
        optionsBuilder.UseSqlServer(builder.ConnectionString);
        // Absolute path
        optionsBuilder.UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Dev\yh\datalagring\IssuesGalore\IssuesGalore\Context\issues_galore_db.mdf;Integrated Security=True;Connect Timeout=30");
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
