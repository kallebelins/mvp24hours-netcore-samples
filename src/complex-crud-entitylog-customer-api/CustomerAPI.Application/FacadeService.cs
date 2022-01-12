using CustomerAPI.Core.Contract.Logic;
using Mvp24Hours.Infrastructure.Helpers;

namespace CustomerAPI.Application
{
    /// <summary>
    /// Provides all services available for use in this project
    /// </summary>
    public class FacadeService
    {
        #region [ Services ]
        /// <summary>
        /// <see cref="CustomerAPI.Core.Contract.Logic.ICustomerService"/>
        /// </summary>
        public static ICustomerService CustomerService
        {
            get { return ServiceProviderHelper.GetService<ICustomerService>(); }
        }
        /// <summary>
        /// <see cref="CustomerAPI.Core.Contract.Logic.ICustomerContactService"/>
        /// </summary>
        public static ICustomerContactService CustomerContactService
        {
            get { return ServiceProviderHelper.GetService<ICustomerContactService>(); }
        }
        #endregion
    }
}
