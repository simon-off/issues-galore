using IssuesGalore.Models;
using IssuesGalore.Models.Entities;
using Microsoft.EntityFrameworkCore;
using WpfApp.Context;

namespace IssuesGalore.Services;

internal class TicketService
{
    private readonly DataContext _context = new();

    public async Task<TicketEntity> CreateAsync(NewTicket newTicket)
    {
        var ticketEntity = new TicketEntity
        {
            CustomerId = newTicket.CustomerId,
            Subject = newTicket.Subject,
            Description = newTicket.Description,
        };

        await _context.AddAsync(ticketEntity);
        await _context.SaveChangesAsync();

        return ticketEntity;
    }

    public async Task<IEnumerable<TicketEntity>> GetAllAsync()
    {
        return await _context.Tickets
            .Include(x => x.Customer)
            .Include(x => x.Status)
            .OrderBy(x => x.WhenCreated)
            .ToListAsync();
    }

    public async Task<TicketEntity> GetAsync(Guid id)
    {
        return await _context.Tickets
            .Include(x => x.Customer)
            .Include(x => x.Status)
            .Include(x => x.Comments)
            .FirstOrDefaultAsync(x => x.Id == id) ?? null!;
    }

    public async Task<TicketEntity> UpdateAsync(Guid id, int statusId)
    {
        var ticketEntity = await GetAsync(id);

        // Return null if invalid statusId
        if (!await _context.Statuses.AnyAsync(x => x.Id == statusId))
            return null!;

        ticketEntity.StatusId = statusId;

        _context.Update(ticketEntity);
        await _context.SaveChangesAsync();

        return ticketEntity;
    }

    public async Task<TicketEntity> DeleteAsync(Guid id)
    {
        var ticketEntity = await GetAsync(id);

        _context.Remove(ticketEntity);
        await _context.SaveChangesAsync();

        return ticketEntity;
    }
}