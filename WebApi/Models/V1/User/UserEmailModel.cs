﻿using Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace WebApi.V1
{
    public class UserEmailModel
    {
        /// <summary>
        /// Email.
        /// </summary>
        /// <example>ivan.ivanov@mail.com</example>
        [EmailAddress]
        public required string Email { get; set; }

        /// <summary>
        /// Old password.
        /// </summary>
        /// <example>m4A0?Edis66a</example>
        [StringLength(UserConstants.RawPasswordMaxLength)]
        public required string Password { get; set; }
    }
}