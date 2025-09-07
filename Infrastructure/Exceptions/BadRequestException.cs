using Microsoft.AspNetCore.Http;

namespace Infrastructure
{
    public class BadRequestException : HttpException
    {
        public BadRequestException(string? message = null)
            : base(StatusCodes.Status400BadRequest, message)
        {

        }

        public BadRequestException(Exception exception)
            : base(StatusCodes.Status400BadRequest, exception)
        {

        }
    }
}