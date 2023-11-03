namespace UserJourney.Repositories.Dto
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using UserJourney.Repositories.Constants;

    public class RegisterDto
    {
        [StringLength(50)]
        [Required(ErrorMessage = SystemMessage.RequiredInput)]
        [RegularExpression(RegularExpressions.HtmlTag, ErrorMessage = SystemMessage.InvalidInput)]
        public string FirstName { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = SystemMessage.RequiredInput)]
        [RegularExpression(RegularExpressions.HtmlTag, ErrorMessage = SystemMessage.InvalidInput)]
        public string LastName { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = SystemMessage.RequiredInput)]
        [RegularExpression(RegularExpressions.Email, ErrorMessage = SystemMessage.EmailErrorMessage)]
        public string Email { get; set; }

        [StringLength(50)]
        [RegularExpression(RegularExpressions.PhoneNumber, ErrorMessage = SystemMessage.PhoneNumberErrorMessage)]
        public string PhoneNumber { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = SystemMessage.RequiredInput)]
        [RegularExpression(RegularExpressions.Password, ErrorMessage = SystemMessage.InvalidPassword)]
        public string Password { get; set; }
    }
}
