namespace IssuesGalore.Models;

internal class NewComment
{
    public NewComment(string text, Guid ticketId)
    {
        Text = text;
        TicketId = ticketId;
    }

    public string Text { get; set; }
    public Guid TicketId { get; set; }
}
