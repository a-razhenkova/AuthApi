namespace WebApi
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SkipLogAttribute : Attribute
    {
        public SkipLogAttribute()
        {

        }
    }
}