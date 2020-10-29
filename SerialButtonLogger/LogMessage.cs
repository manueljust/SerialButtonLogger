namespace SerialButtonLogger
{
    public class LogMessage
    {
        public enum Severity
        {
            Warning,
            Info,
            Error,
            Log
        }

        public Severity MessageSeverity { get; set; } = Severity.Info;

        public string Message { get; set; } = string.Empty;
    }
}
