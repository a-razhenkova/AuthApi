namespace Infrastructure
{
    public enum UserRoles
    {
        Administrator = 0,
        Employee = 1,
        Customer = 2
    }

    public enum UserStatuses
    {
        Active = 0,
        Restricted = 1,
        Blocked = 2,
        Disabled = 3
    }

    public enum UserStatusReasons
    {
        None = 0,
        MaxWrongLoginAttemptsReached = 1,
        NewUser = 2,
        EmailChanged = 3,
    }
}