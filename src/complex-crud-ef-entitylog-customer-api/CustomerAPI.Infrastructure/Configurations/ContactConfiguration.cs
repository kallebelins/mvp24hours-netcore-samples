using CustomerAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerAPI.Infrastructure.Builders
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.ToTable("Contact", "dbo");

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
                .Property(p => p.CustomerId)
                .IsRequired();
            builder
                .Property(p => p.Type)
                .HasConversion<string>()
                .IsRequired();
            builder
                .Property(p => p.Description)
                .HasMaxLength(255)
                .IsRequired();
            builder
                .Property(p => p.Active)
                .IsRequired();
            #endregion

            #region [ Relationships members ]
            builder
                .HasOne(p => p.Customer)
                .WithMany(p => p.Contacts)
                .HasForeignKey(p => p.CustomerId);
            #endregion
        }
    }
}
