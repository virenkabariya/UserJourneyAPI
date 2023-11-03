namespace UserJourney.Repositories.Dto
{
    public class TokenDto
    {
        public string AccessToken { get; set; }

        public DateTime ExpireDateTime { get; set; }

        public string TokenType { get; set; }
    }
}
