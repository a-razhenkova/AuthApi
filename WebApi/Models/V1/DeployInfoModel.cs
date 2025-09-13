namespace WebApi.V1
{
    public class DeployInfoModel
    {
        /// <summary>
        /// The application version.
        /// </summary>
        /// <example>1.0.0.0</example>
        public required string Version { get; set; }

        /// <summary>
        /// The environment.
        /// </summary>
        /// <example>Development</example>
        public required string Environment { get; set; }

        /// <summary>
        /// Server timestamp when the information was captured.
        /// </summary>
        public required DateTime ServerTimestamp { get; set; }
    }
}