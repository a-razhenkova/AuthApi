namespace WebApi
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SensitiveDataAttribute : Attribute
    {
        public SensitiveDataAttribute(bool isRequestSensitive = true,
                                     bool isResponseSensitive = false)
        {
            IsRequestSensitive = isRequestSensitive;
            IsResponseSensitive = isResponseSensitive;
        }

        public bool IsRequestSensitive { get; init; }

        public bool IsResponseSensitive { get; init; }
    }
}