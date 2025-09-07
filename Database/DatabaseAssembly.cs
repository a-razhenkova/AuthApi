using System.Reflection;

namespace Database
{
    public static class DatabaseAssembly
    {
        public static Assembly GetExecutingAssembly()
            => Assembly.GetExecutingAssembly();
    }
}