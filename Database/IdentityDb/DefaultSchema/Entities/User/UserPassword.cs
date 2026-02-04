using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.IdentityDb.DefaultSchema
{
    [Table("user_password", Schema = DefaultSchemaSettings.SchemaName)]
    public class UserPassword : EntityBase
    {
        [Required]
        [ForeignKey(nameof(User))]
        [Column("user_id", Order = 2)]
        public long UserId { get; set; }

        [Required]
        [Column("password", Order = 3)]
        [MaxLength(UserConstants.PasswordHashMaxLength), Unicode(false)]
        public string Value { get; set; }

        [Required]
        [Column("secret", Order = 4)]
        [MaxLength(UserConstants.PasswordSecretMaxLength), Unicode(false)]
        public string Secret { get; set; }

        [Required]
        [Column("last_changed_timestamp", Order = 5)]
        public DateTime LastChangedTimestamp { get; set; }

        #region Relationships

        [InverseProperty(nameof(User.Password))]
        public virtual User User { get; set; }

        #endregion
    }
}