using CustomerAPI.Core.Entities;
using Mvp24Hours.Core.Enums.Infrastructure;
using Mvp24Hours.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAPI.Infrastructure.Data
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Abbreviation for Entity Framework Database")]
    public static class EFDBContextSeed
    {
        public static async Task SeedAsync(EFDBContext dbContext)
        {
            // adicionar processamento de dados iniciais para carga
            TelemetryHelper.Execute(TelemetryLevels.Information, "efdbcontextseed-seedasync", $"Seed database associated with context {dbContext.GetType().Name}");

            if (!dbContext.Customer.Any())
            {
                dbContext.Customer.AddRange(GetCustomers());
                await dbContext.SaveChangesAsync();
            }

            await Task.CompletedTask;
        }

        private static List<Customer> GetCustomers()
        {
            return new List<Customer>
            {
                new Customer
                {
                    Created = DateTime.Now,
                    Name = "Cherokee Macdonald",
                    Active = true,
                    Note = "Customer charged via standard charge.",
                    Contacts = new List<Contact>
                    {
                        new Contact
                        {
                            Created = DateTime.Now,
                            Description = "(800) 997-348",
                            Active = true
                        }
                    }
                },
                new Customer
                {
                    Created = DateTime.Now,
                    Name = "Jonah Harvey",
                    Active = true,
                    Note = "Customer charged via standard charge.",
                    Contacts = new List<Contact>
                    {
                        new Contact
                        {
                            Created = DateTime.Now,
                            Description = "1-392-598-4254",
                            Active = true
                        }
                    }
                }
            };
        }
    }
}
