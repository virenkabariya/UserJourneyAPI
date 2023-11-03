namespace UserJourney.Repositories.EF
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using UserJourney.Repositories.Constants;

    [Table("Users")]
    public partial class Users
    {
        [Key]
        [Column("UserId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

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
        public String? PhoneNumber { get; set; } = null!;

        [StringLength(50)]
        [Required(ErrorMessage = SystemMessage.RequiredInput)]
        [RegularExpression(RegularExpressions.Password, ErrorMessage = SystemMessage.InvalidPassword)]
        public String Password { get; set; }

        [StringLength(50)]
        public String? PasswordResetToken { get; set; }
        public DateTime? LastTokenCreatedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
