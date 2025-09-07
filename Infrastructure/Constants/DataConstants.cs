﻿namespace Infrastructure
{
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