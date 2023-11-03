namespace UserJourney.Repositories.Dto
{
    using System.ComponentModel.DataAnnotations;
    using UserJourney.Repositories.Constants;

    public class ForgetPasswordDto
    {
        [StringLength(100)]
        [Required(ErrorMessage = SystemMessage.RequiredInput)]
        [RegularExpression(RegularExpressions.Email, ErrorMessage = SystemMessage.EmailErrorMessage)]
        public string Email { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = SystemMessage.RequiredInput)]
        [RegularExpression(RegularExpressions.Password, ErrorMessage = SystemMessage.InvalidPassword)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = SystemMessage.RequiredInput)]
        public string PasswordResetToken { get; set; }
    }
}
