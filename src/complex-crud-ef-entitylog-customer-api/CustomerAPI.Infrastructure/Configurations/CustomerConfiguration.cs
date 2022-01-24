using CustomerAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerAPI.Infrastructure.Builders
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Ignore(p => p.EntityKey);

            // customer->id
            builder
                .HasKey(p => p.Id);
            // contact->created
            builder
                .Property(p => p.Created)
                .IsRequired();
            // contact->createdby
            builder
                .Property(p => p.CreatedBy);
            // contact->modified
            builder
                .Property(p => p.Modified);
            // contact->modifiedby
            builder
                .Property(p => p.ModifiedBy);
            // contact->removed
            builder
                .Property(p => p.Removed);
            // contact->removedby
            builder
                .Property(p => p.RemovedBy);
            // customer->name
            builder
                .Property(p => p.Name)
                .HasMaxLength(50)
                .IsRequired();
            // customer->note
            builder
                .Property(p => p.Note)
                .HasMaxLength(2000);
            // customer->active
            builder
                .Property(p => p.Active)
                .IsRequired();

            // customer->contacts
            builder
                .HasMany(p => p.Contacts)
                .WithOne(p => p.Customer)
                .HasForeignKey(p => p.CustomerId);
        }
    }
}
