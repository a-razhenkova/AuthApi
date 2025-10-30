namespace Infrastructure
{
    public enum ClientStatuses
    {
        Disabled = 0,
        Active = 1,
        Blocked = 2
    }

    public enum ClientStatusReasons
    {
        None = 0,
        MaxWrongLoginAttemptsReached,
        ExpiredSubscription
    }
}