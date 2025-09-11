using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Infrastructure;

namespace Database.AuthDb.DefaultSchema
{
    [Table("document", Schema = DefaultSchemaSettings.SchemaName)]
    public class Document : EntityBase
    {
        [Required]
        [Column("sign_timestamp", Order = 2)]
        public DateTime SignTimestamp { get; set; }

        [Required]
        [Column("name", Order = 3)]
        public string Name { get; set; }

        [Required]
        [Column("checksum", Order = 4)]
        public string Checksum { get; set; }

        [Required]
        [Column("type", Order = 5)]
        public DocumentTypes Type { get; set; }

        #region Relationships

        [InverseProperty(nameof(Subscription.Contract))]
        public virtual Subscription Subscription { get; set; }

        #endregion
    }
}