namespace UserJourney.API.Contracts
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(List<string> to, List<string> cc, string subject, string templateName, Dictionary<string, string> values, byte[] attachment = null, string attachmentName = null);
    }
}
