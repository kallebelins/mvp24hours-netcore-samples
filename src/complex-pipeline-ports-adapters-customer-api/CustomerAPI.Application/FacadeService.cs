using CustomerAPI.Core.Contract.Logic;
using Mvp24Hours.Helpers;

namespace CustomerAPI.Application
{
    /// <summary>
    /// Provides all services available for use in this project
    /// </summary>
    public static class FacadeService
    {
        #region [ Services ]
        /// <summary>
        /// <see cref="CustomerAPI.Core.Contract.Logic.ICustomerService"/>
        /// </summary>
        public static ICustomerService CustomerService
        {
            get { return ServiceProviderHelper.GetService<ICustomerService>(); }
        }
        #endregion
    }
}
