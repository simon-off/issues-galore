using IssuesGalore.Models;
using IssuesGalore.Models.Entities;
using Microsoft.EntityFrameworkCore;
using WpfApp.Context;

namespace IssuesGalore.Services;

internal class CustomerService
{
    private readonly DataContext _context = new();

    public async Task<CustomerEntity> CreateAsync(NewCustomer newCustomer)
    {
        var customerEntity = new CustomerEntity
        {
            FirstName = newCustomer.FirstName,
            LastName = newCustomer.LastName,
            EmailAddress = newCustomer.EmailAddress,
            PhoneNumber = newCustomer.PhoneNumber,
        };

        await _context.AddAsync(customerEntity);
        await _context.SaveChangesAsync();

        return customerEntity;
    }

    public async Task<IEnumerable<CustomerEntity>> GetAllAsync()
    {
        return await _context.Customers.ToListAsync();
    }

    public async Task<CustomerEntity> GetByEmailAsync(string emailAddress)
    {
        return await _context.Customers.FirstOrDefaultAsync(x => x.EmailAddress == emailAddress) ?? null!;
    }
}
