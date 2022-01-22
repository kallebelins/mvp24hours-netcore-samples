using Mvp24Hours.Extensions;
using Mvp24Hours.Infrastructure.Data.MongoDb;
using System.Reflection;

namespace CustomerAPI.Infrastructure.Data
{
    public class CustomerDBContext : Mvp24HoursContext
    {
        #region [ Ctor ]

        public CustomerDBContext(string databaseName, string connectionString)
            : base(databaseName, connectionString)
        {
            this.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        #endregion
    }
}
