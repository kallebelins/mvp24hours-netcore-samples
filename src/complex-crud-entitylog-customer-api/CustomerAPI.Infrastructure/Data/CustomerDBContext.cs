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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }

        #endregion

        #region [ Sets ]

        public virtual DbSet<Customer> Customer { get; set; }

        #endregion
    }
}
