using Microsoft.AspNetCore.Http;

namespace Infrastructure
{
    public class UnauthorizedException : HttpException
    {
        public UnauthorizedException(string? message = null) 
            : base(StatusCodes.Status401Unauthorized, message)
        {

        }
    }
}