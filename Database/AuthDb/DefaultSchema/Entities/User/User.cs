﻿using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.AuthDb.DefaultSchema
{
    [Table("user", Schema = DefaultSchemaSettings.SchemaName)]
    [Index(nameof(ExternalId), IsUnique = true)]
    [Index(nameof(Username), IsUnique = true)]
    public partial class User : EntityBase
    {
        [Required]
        [Column("external_id", Order = 2)]
        [MaxLength(Constants.UidLength), Unicode(false)]
        public string ExternalId { get; set; }

        [Required]
        [Column("username", Order = 3)]
        [MaxLength(UserConstants.UsernameMaxLength), Unicode(false)]
        public string Username { get; set; }

        [Required]
        [Column("role", Order = 5)]
        public UserRoles Role { get; set; }

        [Required]
        [Column("otp_secret", Order = 6)]
        [MaxLength(UserConstants.UserSecretMaxLength), Unicode(false)]
        public string OtpSecret { get; set; }

        [Column("email", Order = 7)]
        [EmailAddress]
        public string Email { get; set; }

        [Column("is_verified", Order = 8)]
        public bool IsVerified { get; set; } = false;

        #region Relationships

        [InverseProperty(nameof(Status.User))]
        public virtual UserStatus Status { get; set; }

        [InverseProperty(nameof(UserPassword.User))]
        public virtual UserPassword Password { get; set; }

        [InverseProperty(nameof(Login.User))]
        public virtual Login Login { get; set; }

        #endregion
    }
}