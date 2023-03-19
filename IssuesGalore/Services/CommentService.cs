using IssuesGalore.Models;
using IssuesGalore.Models.Entities;
using Microsoft.EntityFrameworkCore;
using WpfApp.Context;

namespace IssuesGalore.Services;

internal class CommentService
{
    private readonly DataContext _context = new();

    public async Task<CommentEntity> CreateAsync(NewComment newComment)
    {
        var commentEntity = new CommentEntity
        {
            TicketId = newComment.TicketId,
            Text = newComment.Text
        };

        await _context.AddAsync(commentEntity);
        await _context.SaveChangesAsync();

        return commentEntity;
    }

    public async Task<IEnumerable<CommentEntity>> GetByTicketId(Guid ticketId)
    {
        return await _context.Comments.Where(x => x.TicketId == ticketId).OrderBy(x => x.WhenCreated).ToListAsync();
    }
}
