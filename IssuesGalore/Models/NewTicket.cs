namespace IssuesGalore.Models;

internal class NewTicket
{
    public NewTicket(string subject, string description, Guid customerId)
    {
        Subject = subject;
        Description = description;
        CustomerId = customerId;
    }

    public string Subject { get; set; }
    public string Description { get; set; }
    public Guid CustomerId { get; set; }
}
