namespace WebApi
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SensitiveDataAttribute : Attribute
    {
        public SensitiveDataAttribute()
        {

        }

        public bool IsRequestSensitive { get; set; } = true;

        public bool IsResponseSensitive { get; set; } = false;
    }
}