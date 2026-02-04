using Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.IdentityDb.DefaultSchema
{
    [Table("client_status", Schema = DefaultSchemaSettings.SchemaName)]
    public partial class ClientStatus : EntityBase
    {
        [Required]
        [ForeignKey(nameof(Client))]
        [Column("client_id", Order = 2)]
        public long ClientId { get; set; }

        [Required]
        [Column("status", Order = 3)]
        public ClientStatuses Value { get; set; }

        [Required]
        [Column("reason", Order = 4)]
        public ClientStatusReasons Reason { get; set; }

        [Column("note", Order = 5)]
        public string Note { get; set; }

        #region Relationships

        [InverseProperty(nameof(Client.Status))]
        public virtual Client Client { get; set; }

        #endregion
    }
}