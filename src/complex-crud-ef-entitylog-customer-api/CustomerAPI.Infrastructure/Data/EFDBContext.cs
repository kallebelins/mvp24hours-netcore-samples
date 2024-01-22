using CustomerAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Mvp24Hours.Infrastructure.Data.EFCore;
using System.Reflection;

namespace CustomerAPI.Infrastructure.Data
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Abbreviation for Entity Framework Database")]
    public class EFDBContext : Mvp24HoursContext
    {
        #region [ Ctor ]

        public EFDBContext()
            : base()
        {
        }

        public EFDBContext(DbContextOptions options)
            : base(options)
        {
        }

        #endregion

        #region [ Overrides ]

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        // Enable control for log entities (EntityBaseLog)
        public override bool CanApplyEntityLog => true;

        // You must implement profile or user code recovery here
        public override object EntityLogBy => "SYSTEM";

        #endregion

        #region [ Sets ]

        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Contact> Contact { get; set; }

        #endregion
    }
}
