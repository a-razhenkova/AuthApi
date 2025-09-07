namespace Infrastructure
{
    public enum ClientStatuses
    {
        Active = 0,
        Blocked = 1,
        Disabled = 2,
    }

    public enum ClientStatusReasons
    {
        None = 0,
        MaxWrongLoginAttemptsReached,
        ExpiredSubscription
    }
}