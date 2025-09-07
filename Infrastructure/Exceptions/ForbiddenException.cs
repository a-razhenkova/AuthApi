using Microsoft.AspNetCore.Http;

namespace Infrastructure
{
    public class ForbiddenException : HttpException
    {
        public ForbiddenException(string? message = null)
            : base(StatusCodes.Status403Forbidden, message)
        {

        }
    }
}