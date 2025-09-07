using Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace WebApi.V2
{
    public class UserCredentialsModel
    {
        /// <summary>
        /// User external ID.
        /// </summary>
        /// <example>2a47a4fc-3d90-4ddb-a1ec-a664c0a8a2f3</example>
        public required string UserId { get; set; }

        /// <summary>
        /// One time password (OTP).
        /// </summary>
        /// <example>271967</example>
        [StringLength(Constants.OneTimePasswordLength)]
        public required string OneTimePassword { get; set; }
    }
}