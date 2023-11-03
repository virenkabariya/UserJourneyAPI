namespace UserJourney.API.Code
{
    public class EMailSettings
    {
        public string From { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool SSLEnabled { get; set; }
        public bool BypassAuthentication { get; set; }
        public string EmailTemplatePath { get; set; }
    }
}
