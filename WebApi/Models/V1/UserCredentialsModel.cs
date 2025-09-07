using Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace WebApi.V1
{
    public class UserCredentialsModel
    {
        /// <summary>
        /// Username.
        /// </summary>
        /// <example>ivan.ivanov</example>
        [StringLength(UserConstants.UsernameMaxLength, MinimumLength = UserConstants.UsernameMinLength)]
        [RegularExpression(UserConstants.UsernameRegex)]
        public required string Username { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        /// <example>m4A0?Edis66a</example>
        [StringLength(UserConstants.RawPasswordMaxLength)]
        public required string Password { get; set; }
    }
}