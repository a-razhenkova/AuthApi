using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.AuthDb.DefaultSchema
{
    [Table("client_subscription", Schema = DefaultSchemaSettings.SchemaName)]
    public class ClientSubscription : EntityBase
    {
        [Required]
        [ForeignKey(nameof(Client))]
        [Column("client_id", Order = 2)]
        public long ClientId { get; set; }

        [Required]
        [ForeignKey(nameof(Subscription))]
        [Column("subscription_id", Order = 3)]
        public long SubscriptionId { get; set; }

        #region Relationships

        [InverseProperty(nameof(Client.Subscriptions))]
        public virtual Client Client { get; set; }

        [InverseProperty(nameof(Subscription.ClientSubscriptions))]
        public virtual Subscription Subscription { get; set; }

        #endregion
    }
}