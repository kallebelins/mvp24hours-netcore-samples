using CustomerAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerAPI.Infrastructure.Builders
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customer", "dbo");

            #region [ Primitive members ]
            builder
                .HasKey(p => p.Id);

            // log fields
            builder
                .Property(p => p.Created)
                .IsRequired();
            builder
                .Property(p => p.CreatedBy);
            builder
                .Property(p => p.Modified);
            builder
                .Property(p => p.ModifiedBy);
            builder
                .Property(p => p.Removed);
            builder
                .Property(p => p.RemovedBy);

            // my fields
            builder
                .Property(p => p.Name)
                .HasMaxLength(50)
                .IsRequired();
            builder
                .Property(p => p.Note)
                .HasMaxLength(2000);
            builder
                .Property(p => p.Active)
                .IsRequired();
            #endregion

            #region [ Relationships members ]
            builder
                .HasMany(p => p.Contacts)
                .WithOne(p => p.Customer)
                .HasForeignKey(p => p.CustomerId);
            #endregion
        }
    }
}
