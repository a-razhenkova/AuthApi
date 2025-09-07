using Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace Business
{
    public class UserSearchParams : PaginationParams
    {
        /// <summary>
        /// Username.
        /// </summary>
        [StringLength(UserConstants.UsernameMaxLength)]
        public string? Username { get; set; }

        /// <summary>  
        /// User role.  
        /// </summary>
        public UserRoles? Role { get; set; }

        /// <summary>
        /// User status.
        /// </summary>
        public UserStatuses? Status { get; set; }
    }
}