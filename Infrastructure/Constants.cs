namespace Infrastructure
{
    public static class Constants
    {
        public const string DefaultAssemblyVersion = "1.0.0.0";

        public const int UidLength = 36;
        public const int IpAddressMaxLength = 39;
        public const int OneTimePasswordLength = 6;

        public const string SerilogOutputTemplate = "> {Timestamp:dd.MM.yyyy HH:mm:ss.fff} - {Level:u3}|{MachineName}(v.{Version})|{SourceContext}{NewLine}[{Properties}]{NewLine}Message:{Message:lj}{NewLine}{Exception}";
        
        public const string FreeTextRegex = @"^[a-zA-Z\d\s\-.!?()]*$";
    }

    public static class ClientConstants
    {
        public const int Key = Constants.UidLength;
        public const int Secret = Constants.UidLength;
        public const int Name = Constants.UidLength;

        public const string NameRegex = @"^[a-zA-Z][\w .-]+[a-zA-Z]$";
        public const string StatusNoteRegex = Constants.FreeTextRegex;
    }

    public static class UserConstants
    {
        public const int ExternalIdMaxLength = Constants.UidLength;
        public const int UsernameMinLength = 6;
        public const int UsernameMaxLength = 64;
        public const int UserSecretMaxLength = 64;
        public const int RawPasswordMaxLength = 128;
        public const int PasswordHashMaxLength = 256;
        public const int PasswordSecretMaxLength = 128;

        public const string UsernameRegex = @"^[a-zA-Z][\w.-]+[a-zA-Z]$";
        public const string StatusNoteRegex = Constants.FreeTextRegex;
    }
}