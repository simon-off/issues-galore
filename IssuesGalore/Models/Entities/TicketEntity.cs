using System.ComponentModel.DataAnnotations;

namespace IssuesGalore.Models.Entities;

internal class TicketEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime WhenCreated { get; set; } = DateTime.Now;

    [MaxLength(200)]
    public string Subject { get; set; } = null!;
    public string Description { get; set; } = null!;

    public int StatusId { get; set; } = 1;
    public StatusEntity Status { get; set; } = null!;

    public Guid CustomerId { get; set; }
    public CustomerEntity Customer { get; set; } = null!;

    public ICollection<CommentEntity> Comments { get; set; } = new HashSet<CommentEntity>();
}
