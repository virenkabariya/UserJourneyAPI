using System.ComponentModel.DataAnnotations;
using UserJourney.Repositories.Constants;

namespace UserJourney.Repositories.Dto
{
    public class ResetPasswordDto
    {
        [StringLength(50)]
        [Required(ErrorMessage = SystemMessage.RequiredInput)]
        public string OldPassword { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = SystemMessage.RequiredInput)]
        [RegularExpression(RegularExpressions.Password, ErrorMessage = SystemMessage.InvalidPassword)]
        public string NewPassword { get; set; }
    }
}
