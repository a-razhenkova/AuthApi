using Microsoft.AspNetCore.Http;

namespace Infrastructure
{
    public class ConflictException : HttpException
    {
        public ConflictException(string? message = null)
            : base(StatusCodes.Status409Conflict, message)
        {

        }
    }
}