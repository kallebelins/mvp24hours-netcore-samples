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
            // customer->name
            builder
                .Property(p => p.Name)
                .HasMaxLength(50)
                .IsRequired();
            // customer->cellphone
            builder
                .Property(p => p.CellPhone)
                .HasMaxLength(15);
            // customer->email
            builder
                .Property(p => p.Email)
                .HasMaxLength(255);
            // customer->note
            builder
                .Property(p => p.Note)
                .HasMaxLength(2000);
            // customer->active
            builder
                .Property(p => p.Active)
                .IsRequired();
        }
    }
}
