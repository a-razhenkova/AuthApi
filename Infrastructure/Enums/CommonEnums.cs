using System.ComponentModel;

namespace Infrastructure
{
    public enum LoggerContextProperty
    {
        Environment,
        Version,
        MachineName,
        ActionType,
        ActionId,
        TraceId,
        CorrelationId
    }

    public enum LoggerContext
    {
        WebApi,
        HealthCheck,
        DbMigration,
        DbUp
    }

    public enum HealthCheckImpactTag
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum TokenClaim
    {
        // client
        [Description("clientId")]
        ClientId,

        // user
        [Description("userId")]
        UserExternalId,
        [Description("username")]
        Username,
        [Description("userRole")]
        UserRole,
        [Description("userStatus")]
        UserStatus,

        // rights
        [Description("canNotifyParty")]
        CanNotifyParty
    }

    public enum DocumentTypes
    {
        SubscriptionContract
    }
}