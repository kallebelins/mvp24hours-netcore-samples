using CustomerAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Mvp24Hours.Infrastructure.Data.EFCore;
using System.Reflection;

namespace CustomerAPI.Infrastructure.Data
{
    public class CustomerDBContext : Mvp24HoursContext
    {
        #region [ Ctor ]

        public CustomerDBContext()
            : base()
        {
        }

        public CustomerDBContext(DbContextOptions options)
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

        #endregion

        #region [ Sets ]

        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Contact> Contact { get; set; }

        #endregion
    }
}
