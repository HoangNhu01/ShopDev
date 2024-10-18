namespace ShopDev.Constants.Environments
{
    public static class EnvironmentNames
    {
        public const string Production = "prod";
        public const string Development = "Development";
        public const string DevelopmentWSL = "DevelopmentWSL";
        public const string Staging = "stag";
        public const string DockerDev = "DockerDev";

        public static readonly string[] DevelopEnv =
        [
            Development,
            DevelopmentWSL,
            DockerDev,
            Staging
        ];

        public static readonly string[] Productions = [Staging, Production];
    }
}
