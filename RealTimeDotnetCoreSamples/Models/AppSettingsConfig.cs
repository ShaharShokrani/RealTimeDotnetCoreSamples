namespace RealTimeDotnetCoreSamples.Models
{
    public class AppSettingsConfig
    {
        public TypicodeConfig Typicode { get; set; }
        public StackoverflowConfig Stackoverflow { get; set; }
    }

    public class StackoverflowConfig
    {
        public int UsersThreadSleep { get; set; }
    }

    public class TypicodeConfig
    {
        public int UsersThreadSleep { get; set; }
    }
}
