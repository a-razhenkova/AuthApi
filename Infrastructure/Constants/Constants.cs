namespace Infrastructure
{
    public static class Constants
    {
        public const string DefaultAssemblyVersion = "1.0.0.0";

        public const int UidLength = 36;
        public const int IpAddressMaxLength = 39;
        public const int OneTimePasswordLength = 6;

        public const string SerilogOutputTemplate = "> {Timestamp:dd.MM.yyyy HH:mm:ss.fff} - {Level:u3}|{MachineName}(v.{Version})|{SourceContext}{NewLine}[{Properties}]{NewLine}Message:{Message:lj}{NewLine}{Exception}";
        public const string SensitiveDataMask = "******";
        
        public const string FreeTextRegex = @"^[a-zA-Z\d\s\-.!?()]*$";
    }
}