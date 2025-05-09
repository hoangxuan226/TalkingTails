namespace TalkingTails.Business.Models.Setting
{
    public class AzureStorageSettings
    {
        public const string SectionName = "AzureStorage";
        public string ConnectionString { get; init; } = null!;
        public string ContainerName { get; init; } = null!;
    }
}