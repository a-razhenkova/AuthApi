﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.AuthDb.DefaultSchema
{
    [Table("login", Schema = DefaultSchemaSettings.SchemaName)]
    public class Login : EntityBase
    {
        [Required]
        [ForeignKey(nameof(User))]
        [Column("user_id", Order = 2)]
        public long UserId { get; set; }

        [Required]
        [Column("wrong_login_attempts_counter", Order = 3)]
        public int WrongLoginAttemptsCounter { get; set; } = 0;

        [Column("last_login_timestamp", Order = 4)]
        public DateTime? LastLoginDate { get; set; }

        [Column("last_login_ip_address", Order = 5)]
        public string LastLoginIpAddress { get; set; }

        #region Relationships

        [InverseProperty(nameof(User.Login))]
        public virtual User User { get; set; }

        #endregion
    }
}