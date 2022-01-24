using CustomerAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerAPI.Infrastructure.Builders
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            // contact->id
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
            // contact->customerId
            builder
                .Property(p => p.CustomerId)
                .IsRequired();
            // contact->type
            builder
                .Property(p => p.Type)
                .HasConversion<string>()
                .IsRequired();
            // contact->description
            builder
                .Property(p => p.Description)
                .HasMaxLength(255)
                .IsRequired();
            // contact->active
            builder
                .Property(p => p.Active)
                .IsRequired();

            // contact->customer
            builder
                .HasOne(p => p.Customer)
                .WithMany(p => p.Contacts)
                .HasForeignKey(p => p.CustomerId);
        }
    }
}
