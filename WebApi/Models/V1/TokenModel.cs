namespace WebApi.V1
{
    public class TokenModel
    {
        /// <summary>
        /// Access token.
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIyYTQ3YTRmYy0zZDkwLTRkZGItYTFlYy1hNjY0YzBhOGEyZjMiLCJ1c2VybmFtZSI6Iml2YW4uaXZhbm92IiwidXNlclJvbGUiOiJBZG1pbmlzdHJhdG9yIiwidXNlclN0YXR1cyI6IkFjdGl2ZSIsIm5iZiI6MTc0OTUwMTEzNCwiZXhwIjoxNzQ5NTczMTM0LCJpYXQiOjE3NDk1MDExMzQsImlzcyI6IkQxQ0NFNUE5RkQ5ODBCOTlCMkZDM0FGQjg4MThDQTZBRUNBNEU5RDFCRUI0N0FGMUM1OTc4REMyMEVCNTJCMEMiLCJhdWQiOiI4MTA0NTJhODA4YTIyMGM1MjQxNDUyYWJjMDQzNzZlNjZhMWJiNDE1NGU5NTRlYjQ3MjRjNGI4ZmY5Mzk5YmI2In0.f9jOUW8SjO9lFqEpFNzAAZnBgS4k6pqv8QUNx8y9aJg</example>
        public required string AccessToken { get; set; }
    }
}