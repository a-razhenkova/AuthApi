namespace WebApi.V1
{
    public class ClientSubscriptionModel
    {
        /// <summary>
        /// The expiration date of the subscription.
        /// </summary>
        public DateTime ExpirationDate { get; set; } = DateTime.Now;

        /// <summary>
        /// The contract identifier associated with the subscription.
        /// </summary>
        public long ContractId { get; set; } = 0;

        /// <summary>
        /// The name of the contract associated with the subscription.
        /// </summary>
        public string ContractName { get; set; } = string.Empty;
    }
}