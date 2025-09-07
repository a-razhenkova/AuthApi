using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.AuthDb.DefaultSchema
{
    [Table("client_subscription", Schema = DefaultSchemaSettings.SchemaName)]
    public class ClientSubscription : EntityBase
    {
        [ForeignKey(nameof(Client))]
        [Column("client_id", Order = 2)]
        public long ClientId { get; set; }

        [Required]
        [Column("expiration_date", Order = 3)]
        public DateTime ExpirationDate { get; set; }

        // TODO: add contract

        #region Relationships

        [InverseProperty(nameof(Client.Subscription))]
        public virtual Client Client { get; set; }

        #endregion
    }
}