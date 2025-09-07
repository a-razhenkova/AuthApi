using Microsoft.AspNetCore.Http;

namespace Infrastructure
{
    public class NotFoundException : HttpException
    {
        public NotFoundException(string? message = null)
            : base(StatusCodes.Status404NotFound, message)
        {

        }
    }
}