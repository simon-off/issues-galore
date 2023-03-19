namespace IssuesGalore.Models.Entities;

internal class CommentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime WhenCreated { get; set; } = DateTime.Now;

    public string Text { get; set; } = null!;

    public Guid TicketId { get; set; }
    public TicketEntity Ticket { get; set; } = null!;
}
