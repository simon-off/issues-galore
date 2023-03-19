using IssuesGalore.Models.Entities;
using Microsoft.EntityFrameworkCore;
using WpfApp.Context;

namespace IssuesGalore.Services;

internal class StatusService
{
    private readonly DataContext _context = new();

    public async Task InitializeAsync()
    {
        var statuses = new List<StatusEntity>()
            {
                new StatusEntity { Id = 1, Name = "Not started"},
                new StatusEntity { Id = 2, Name = "In progress" },
                new StatusEntity { Id = 3, Name = "Closed" },
            };

        await _context.AddRangeAsync(statuses);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<StatusEntity>> GetAllAsync()
    {
        return await _context.Statuses.ToListAsync();
    }

    public async Task<StatusEntity> GetAsync(int id)
    {
        return await _context.Statuses.FirstOrDefaultAsync(x => x.Id == id) ?? null!;
    }
}
