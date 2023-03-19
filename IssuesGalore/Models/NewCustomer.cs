using System.Globalization;

namespace IssuesGalore.Models;

internal class NewCustomer
{
    public NewCustomer(string fName, string lName, string email, string phone)
    {
        FirstName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(fName);
        LastName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(lName);
        EmailAddress = email.ToLowerInvariant();
        PhoneNumber = phone;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
}
