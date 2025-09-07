using System.Reflection;

namespace Business
{
    public static class BusinessAssembly
    {
        public static string GetName()
            => Assembly.GetExecutingAssembly()?.GetName().Name ?? "unknown";
    }
}