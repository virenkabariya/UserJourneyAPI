namespace UserJourney.Repositories.CustomException
{
    using Microsoft.AspNetCore.Mvc;

    public class CustomExceptionDetails : ProblemDetails
    {
        public string AdditionalInfo { get; set; }
    }
}
