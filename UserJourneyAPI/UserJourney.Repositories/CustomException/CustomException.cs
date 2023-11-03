namespace UserJourney.Repositories.CustomException
{
    public class CustomException : Exception
    {
        public CustomException(string detail)
        {
            Type = "UserJourney-Exception";
            Detail = detail;
            Title = "Custom UserJourney- Exception";
            AdditionalInfo = "Maybe you can try again in a bit?";
            Instance = "UserJourney--Instance";
        }
        public string AdditionalInfo { get; set; }
        public string Type { get; set; }
        public string Detail { get; set; }
        public string Title { get; set; }
        public string Instance { get; set; }
    }
}
