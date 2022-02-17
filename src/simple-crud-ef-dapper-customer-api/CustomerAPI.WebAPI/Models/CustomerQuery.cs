namespace CustomerAPI.WebAPI.Models
{
    /// <summary>
    /// Template for customer filter
    /// </summary>
    public class CustomerQuery
    {
        /// <summary>
        /// Client name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Indicates whether the client is active/inactive
        /// </summary>
        public bool? Active { get; set; }
    }
}
