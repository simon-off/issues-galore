using IssuesGalore.Models;
using IssuesGalore.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WpfApp.Context;

namespace IssuesGalore.Services;

internal class UIService
{
    private readonly int cmdPad = 16;
    private readonly DataContext _context = new();
    private readonly TicketService _ticketService = new();
    private readonly CustomerService _customerService = new();
    private readonly CommentService _commentService = new();
    private readonly StatusService _statusService = new();

    /// <summary>
    ///     Writes the specified data, followed by the current line terminator, to the standard output stream, while wrapping lines that would otherwise break words.
    ///     https://stackoverflow.com/questions/20534318/make-console-writeline-wrap-words-instead-of-letters
    /// </summary>
    /// <param name="paragraph">The value to write.</param>
    /// <param name="tabSize">The value that indicates the column width of tab characters.</param>
    public static void WriteLineWordWrap(string paragraph, int tabSize = 8)
    {
        string[] lines = paragraph
            .Replace("\t", new String(' ', tabSize))
            .Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

        for (int i = 0; i < lines.Length; i++)
        {
            string process = lines[i];
            List<string> wrapped = new();

            while (process.Length > Console.WindowWidth)
            {
                int wrapAt = process.LastIndexOf(' ', Math.Min(Console.WindowWidth - 1, process.Length));
                if (wrapAt <= 0) break;

                wrapped.Add(process.Substring(0, wrapAt));
                process = process.Remove(0, wrapAt + 1);
            }

            foreach (string wrap in wrapped)
            {
                Console.WriteLine(wrap);
            }

            Console.WriteLine(process);
        }
    }

    private static string GetValidInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();

            if (input == null || input == "")
            {
                Console.WriteLine("! Please provide a value");
                continue;
            }

            return input.Trim();
        }
    }

    public static void PrintBanner(string text)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"╔{new string('═', Console.BufferWidth - 2)}╗");
        builder.AppendLine($"║ {text} {new string(' ', Console.BufferWidth - text.Length - 4)}║");
        builder.AppendLine($"╚{new string('═', Console.BufferWidth - 2)}╝");

        Console.Write(builder.ToString());
    }

    public static void PrintSectionBanner(string text)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"╔{new string('═', Console.BufferWidth - 2)}╗");
        builder.AppendLine($"║ {text} {new string(' ', Console.BufferWidth - text.Length - 4)}║");
        builder.AppendLine($"╙{new string('─', Console.BufferWidth - 2)}╜");

        Console.Write(builder.ToString());
    }

    public static void PrintDivider(char symbol)
    {
        Console.WriteLine(new string(symbol, Console.BufferWidth));
    }

    public async Task Initialize()
    {
        Console.WriteLine("Loading...");

        if (!await _context.Statuses.AnyAsync())
        {
            Console.WriteLine("First time startup - Initializing statuses...");
            await _statusService.InitializeAsync();
        }
    }

    public async Task NewTicketMenu()
    {
        Console.Clear();
        PrintSectionBanner("Customer details");
        var emailAddress = GetValidInput("Email address: ");
        var customer = await _customerService.GetByEmailAsync(emailAddress);

        if (customer == null)
        {
            var firstName = GetValidInput("First name: ");
            var lastName = GetValidInput("Last name: ");
            var phoneNumber = GetValidInput("PhoneNumber: ");

            customer = await _customerService.CreateAsync(new NewCustomer(firstName, lastName, emailAddress, phoneNumber));
        }

        Console.WriteLine();
        Console.WriteLine($"* Welcome {customer.FirstName}!");
        Console.WriteLine();
        PrintSectionBanner("Ticket details");

        var subject = GetValidInput("Ticket subject: ");
        var description = GetValidInput("Ticket description: ");

        var ticket = await _ticketService.CreateAsync(new NewTicket(subject, description, customer.Id));

        if (ticket != null)
        {
            Console.WriteLine();
            Console.WriteLine($"* Successfully created ticket: {ticket.Id}");
            Console.WriteLine("* Press any key to return to the main menu");
        }
    }

    public List<Guid> PrintTickets(IEnumerable<TicketEntity> tickets)
    {
        var padding = 3;
        var sizes = new
        {
            Number = tickets.Count().ToString().Length + padding,
            Subject = tickets.Max(x => x.Subject.Length) + padding,
            Status = tickets.Max(x => x.Status.Name.Length) + padding,
            Customer = tickets.Max(x => $"{x.Customer.FirstName} {x.Customer.LastName}".Length) + padding,
            Created = tickets.Max(x => x.WhenCreated.ToString().Length) + padding,
        };
        var combinedSize = sizes.Number + sizes.Subject + sizes.Status + sizes.Customer + sizes.Created;

        Console.WriteLine(
            "│ #".PadRight(sizes.Number) +
            "│ Subject".PadRight(sizes.Subject) +
            "│ Status".PadRight(sizes.Status) +
            "│ Customer".PadRight(sizes.Customer) +
            "│ Created".PadRight(sizes.Created));
        Console.WriteLine(
            "├".PadRight(sizes.Number, '─') +
            "┼".PadRight(sizes.Subject, '─') +
            "┼".PadRight(sizes.Status, '─') +
            "┼".PadRight(sizes.Customer, '─') +
            "┼".PadRight(Console.BufferWidth - combinedSize + sizes.Created, '─'));

        var number = 1;
        var idList = new List<Guid>();
        foreach (var ticket in tickets)
        {
            idList.Add(ticket.Id);
            Console.WriteLine(
                $"│ {number}".PadRight(sizes.Number) +
                $"│ {ticket.Subject}".PadRight(sizes.Subject) +
                $"│ {ticket.Status.Name}".PadRight(sizes.Status) +
                $"│ {ticket.Customer.FirstName} {ticket.Customer.LastName}".PadRight(sizes.Customer) +
                $"│ {ticket.WhenCreated}".PadRight(sizes.Created));
            number++;
        }
        Console.WriteLine(
            "│".PadRight(sizes.Number) +
            "│".PadRight(sizes.Subject) +
            "│".PadRight(sizes.Status) +
            "│".PadRight(sizes.Customer) +
            "│");

        return idList;
    }

    public async Task UpdateTicketStatus(Guid ticketId, string args)
    {
        if (args.IsNullOrEmpty())
        {
            Console.WriteLine("! Please supply a new status ID");
            Console.ReadKey();
            return;
        }

        if (!int.TryParse(args.Trim(), out int statusId))
        {
            Console.WriteLine("! Could not parse status ID");
            Console.ReadKey();
            return;
        }

        if (await _statusService.GetAsync(statusId) == null)
        {
            Console.WriteLine($"! Status ID: {statusId} does not exist");
            Console.ReadKey();
            return;
        }

        if (await _ticketService.UpdateAsync(ticketId, statusId) == null)
        {
            Console.WriteLine("! Status update failed");
            Console.ReadKey();
            return;
        }
    }

    public async Task AddComment(Guid ticketId, string args)
    {
        if (args.IsNullOrEmpty())
        {
            Console.WriteLine("! Please supply a comment");
            Console.ReadKey();
            return;
        }

        if (await _commentService.CreateAsync(new NewComment(args.Trim(), ticketId)) == null)
        {
            Console.WriteLine("! Falied to add comment");
            Console.ReadKey();
            return;
        }
    }

    public async Task<bool> DeleteTicket(Guid ticketId)
    {
        Console.Write("! Are you sure you want to delete this ticket? (y/n): ");
        while (true)
        {
            var answer = Console.ReadLine()?.Trim().ToLower();
            if (answer == "y" || answer == "yes")
            {
                var deletedTicket = await _ticketService.DeleteAsync(ticketId);
                Console.WriteLine($"* Successfully deleted ticket: {deletedTicket.Id}");
                Console.ReadKey();
                return true;
            }

            Console.WriteLine("* Deletion aborted");
            Console.ReadKey();
            return false;
        }
    }

    public async Task TicketDetails(List<Guid> idList, string inputNumber)
    {
        if (inputNumber.IsNullOrEmpty())
        {
            Console.WriteLine("! Please supply a ticket number");
            Console.ReadKey();
            return;
        }

        if (!int.TryParse(inputNumber.Trim(), out int ticketNumber))
        {
            Console.WriteLine("! Could not parse ticket number");
            Console.ReadKey();
            return;
        }

        Guid ticketId;
        try
        {
            ticketId = idList.ElementAt(ticketNumber - 1);
        }
        catch
        {
            Console.WriteLine($"! Ticket number is out of range");
            Console.ReadKey();
            return;
        }

        while (true)
        {
            var ticket = await _ticketService.GetAsync(ticketId);

            if (ticket == null)
            {
                Console.WriteLine("! Failed to fetch ticket");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            PrintSectionBanner("Ticket details");
            Console.WriteLine("ID:".PadRight(cmdPad) + $"{ticket.Id}");
            Console.WriteLine("Customer:".PadRight(cmdPad) +
                $"{ticket.Customer.FirstName} " +
                $"{ticket.Customer.LastName} " +
                $"| {ticket.Customer.EmailAddress} " +
                $"| {ticket.Customer.PhoneNumber}");
            Console.WriteLine("Created:".PadRight(cmdPad) + $"{ticket.WhenCreated}");
            Console.WriteLine("Status:".PadRight(cmdPad) + $"{ticket.Status.Id}. {ticket.Status.Name}");
            PrintDivider('─');
            Console.WriteLine(ticket.Subject);
            Console.WriteLine(new string('~', ticket.Subject.Length));
            WriteLineWordWrap(ticket.Description);
            Console.WriteLine();

            PrintSectionBanner("Comments");

            var comments = await _commentService.GetByTicketId(ticket.Id);

            if (!comments.Any())
            {
                Console.WriteLine("No comments added yet");
                Console.WriteLine();
            }
            foreach (var comment in comments)
            {
                WriteLineWordWrap($"{comment.Text}");
                Console.WriteLine($"{comment.WhenCreated}");
                Console.WriteLine();
            }

            PrintSectionBanner("Commands");
            Console.Write("status <nr>".PadRight(cmdPad) + "change ticket status ");
            foreach (var status in await _statusService.GetAllAsync())
            {
                Console.Write($"| {status.Id}: {status.Name} ");
            }
            Console.WriteLine();
            Console.WriteLine("comment <text>".PadRight(cmdPad) + "add comment to ticket");
            Console.WriteLine("delete".PadRight(cmdPad) + "delete ticket");
            Console.WriteLine("exit".PadRight(cmdPad) + "back to main menu");
            PrintDivider('─');

            Console.Write("> ");
            var input = Console.ReadLine();
            if (input == null || input == "")
                continue;
            var command = input.Split(' ')[0].Trim().ToLower();
            var args = string.Join(' ', input.Split().Skip(1));

            switch (command)
            {
                case "status":
                    await UpdateTicketStatus(ticket.Id, args);
                    break;
                case "comment":
                    await AddComment(ticketId, args);
                    break;
                case "delete":
                    if (await DeleteTicket(ticketId))
                        return;
                    break;
                case "exit":
                    return;
                default:
                    Console.WriteLine("! Invalid command");
                    Console.ReadKey();
                    break;
            }
        }
    }

    public async Task TicketsOverview()
    {
        while (true)
        {
            Console.Clear();
            PrintBanner("Tickets overview");
            var tickets = await _ticketService.GetAllAsync();
            List<Guid> idList = new();
            if (tickets.IsNullOrEmpty())
            {
                Console.WriteLine("No tickets added yet");
                Console.WriteLine();
            }
            else
            {
                idList = PrintTickets(tickets);
            }
            PrintSectionBanner("Commands");
            if (!tickets.IsNullOrEmpty())
            {
                Console.WriteLine("open <#>".PadRight(cmdPad) + "show ticket details");
            }
            Console.WriteLine("new".PadRight(cmdPad) + "add new ticket");
            Console.WriteLine("exit".PadRight(cmdPad) + "exit the app");
            PrintDivider('─');

            Console.Write("> ");
            var input = Console.ReadLine();
            if (input == null || input == "")
                continue;
            var command = input.Split(' ')[0].Trim().ToLower();
            var args = string.Join(' ', input.Split().Skip(1));

            switch (command)
            {
                case "open":
                    if (idList.IsNullOrEmpty())
                        break;
                    await TicketDetails(idList, args);
                    break;
                case "new":
                    await NewTicketMenu();
                    break;
                case "exit":
                    return;
                default:
                    Console.WriteLine("! Invalid command");
                    Console.ReadKey();
                    break;
            }
        }
    }
}
