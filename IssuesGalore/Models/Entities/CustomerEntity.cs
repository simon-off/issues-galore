using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssuesGalore.Models.Entities;

[Index(nameof(EmailAddress), IsUnique = true)]
internal class CustomerEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [MaxLength(100)]
    public string LastName { get; set; } = null!;

    [Column(TypeName = "varchar(320)")]
    public string EmailAddress { get; set; } = null!;

    [Column(TypeName = "varchar(20)")]
    public string PhoneNumber { get; set; } = null!;

    public ICollection<TicketEntity> Tickets { get; set; } = new HashSet<TicketEntity>();
}