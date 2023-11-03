namespace UserJourney.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Net.Http.Headers;
    using System.Globalization;
    using System.IdentityModel.Tokens.Jwt;
    using UserJourney.API.Contracts;
    using UserJourney.API.Services;
    using UserJourney.Repositories.ApiModels;
    using UserJourney.Repositories.Constants;
    using UserJourney.Repositories.Contracts;
    using UserJourney.Repositories.Dto;
    using UserJourney.Repositories.EF;

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IProjectUnitOfWork _unitOfWork { get; set; }
        private IGenericRepository<Users> _userRepository;
        private IConfiguration _configuration;
        private IEmailService _emailService;

        public UserController(IProjectUnitOfWork unitOfWork, IConfiguration configuration, IEmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _userRepository = _unitOfWork.GetRepository<Users>();

        }

        [HttpPost]
        [Route("Login")]
        public async Task<ApiResponse<TokenDto>> Login([FromBody] LoginDto model)
        {
            var apiResponse = new ApiResponse<TokenDto>();
            try
            {
                TokenDto tokenModel = new();
                var user = await _userRepository.GetFirstAsync(u => u.Email == model.Email);

                if (user != null && user.UserId > 0)
                {
                    if (user.Password == EncryptionDecryption.GetEncrypt(model.Password))
                    {
                        var authClaims = new Dictionary<string, object>();
                        authClaims.Add("UserId", user.UserId.ToString());
                        authClaims.Add(JwtRegisteredClaimNames.Sub, _configuration["JwtConfig:Subject"]);
                        authClaims.Add(JwtRegisteredClaimNames.Email, model.Email);
                        authClaims.Add(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());

                        tokenModel.AccessToken = JwtService.GenerateSecurityToken(authClaims);
                        tokenModel.ExpireDateTime = DateTime.Now.AddMinutes(90);
                        tokenModel.TokenType = _configuration["JwtConfig:Type"];
                        return apiResponse.HandleResponse(tokenModel);
                    }
                    else
                    {
                        return apiResponse.HandleBadRequest(SystemMessage.InvalidUserNameandPassword);
                    }
                }
                else
                {
                    return apiResponse.HandleBadRequest(SystemMessage.InvalidUserNameandPassword);
                }
            }
            catch (Exception ex)
            {
                return apiResponse.HandleException(ex.Message);
            }
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ApiResponse<string>> Registration([FromBody] RegisterDto model)
        {
            var apiResponse = new ApiResponse<string>();
            try
            {
                var user = await _userRepository.GetFirstAsync(u => u.Email == model.Email);

                if (user != null && user.UserId > 0)
                {
                    return apiResponse.HandleBadRequest(SystemMessage.EmailDuplicateMessage);
                }
                else
                {
                    user = new Users();
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.Password = EncryptionDecryption.GetEncrypt(model.Password);
                    user.PhoneNumber = model.PhoneNumber;
                    user.CreatedDate = DateTime.Now;
                    await _userRepository.CreateAsync(user);
                    await _unitOfWork.SaveAsync();
                    return apiResponse.HandleResponse(SystemMessage.UserRegistrationMessage);
                }
            }
            catch (Exception ex)
            {
                return apiResponse.HandleException(ex.Message);
            }
        }

        [HttpGet]
        [Route("GeneratePasswordResetToken")]
        public async Task<ApiResponse<string>> GeneratePasswordResetToken(string email)
        {
            var apiResponse = new ApiResponse<string>();
            try
            {
                var user = await _userRepository.GetFirstAsync(u => u.Email == email);

                if (user != null && user.UserId > 0)
                {
                    var rand = new Random();
                    string passwordResetCode =
                        Convert.ToInt32(Convert.ToInt32(Math.Floor(Convert.ToDouble(1 + (9 * rand.NextDouble()))))).ToString() +
                        Convert.ToInt32(Convert.ToInt32(Math.Floor(Convert.ToDouble(10 * rand.NextDouble()))))
                            .ToString() +
                        Convert.ToInt32(Convert.ToInt32(Math.Floor(Convert.ToDouble(10 * rand.NextDouble()))))
                            .ToString() +
                        Convert.ToInt32(Convert.ToInt32(Math.Floor(Convert.ToDouble(10 * rand.NextDouble()))))
                            .ToString() +
                        Convert.ToInt32(Convert.ToInt32(Math.Floor(Convert.ToDouble(10 * rand.NextDouble()))))
                            .ToString() + Convert
                            .ToInt32(Convert.ToInt32(Math.Floor(Convert.ToDouble(10 * rand.NextDouble()))))
                            .ToString();

                    var keyValues = new Dictionary<string, string>()
                            {
                                {"[@PasswordResetCode]", passwordResetCode},
                                {"[@UserName]", (user.FirstName ?? "") + " " + (user.LastName ?? "")},
                            };

                    string emailTemplateFileName = "ResetPasswordEmailTemplate.html";
                    var success = await _emailService.SendEmailAsync(new List<string>() { user.Email }, new List<string>(), "Password Reset Code", emailTemplateFileName, keyValues);

                    if (success)
                    {
                        user.LastTokenCreatedDate = DateTime.Now;
                        user.PasswordResetToken = passwordResetCode;
                        user.ModifiedDate = DateTime.Now;
                        await _userRepository.UpdateAsync(user);
                        await _unitOfWork.SaveAsync();
                        return apiResponse.HandleException(SystemMessage.PasswordResetTokenSent);
                    }
                    else
                    {
                        return apiResponse.HandleException(SystemMessage.ErrorOnSendingMail);
                    }
                }
                else
                {
                    return apiResponse.HandleException(SystemMessage.InvalidInput);
                }
            }
            catch (Exception ex)
            {
                return apiResponse.HandleException(ex.Message);
            }
        }

        [HttpPost]
        [Route("ForgetPassword")]
        public async Task<ApiResponse<string>> ForgetPassword([FromBody] ForgetPasswordDto model)
        {
            var apiResponse = new ApiResponse<string>();
            try
            {
                var user = await _userRepository.GetFirstAsync(u => u.Email == model.Email);

                if (user != null && user.UserId > 0)
                {
                    if (user.PasswordResetToken != null
                    && user.PasswordResetToken == model.PasswordResetToken
                    && user.LastTokenCreatedDate != null
                    && Convert.ToDateTime(user.LastTokenCreatedDate, CultureInfo.CurrentCulture).AddMinutes(30) > DateTime.Now)
                    {
                        user.Password = EncryptionDecryption.GetEncrypt(model.NewPassword);
                        user.PasswordResetToken = null;
                        user.LastTokenCreatedDate = null;
                        user.ModifiedDate = DateTime.Now;
                        await _userRepository.UpdateAsync(user);
                        await _unitOfWork.SaveAsync();
                        return apiResponse.HandleException(SystemMessage.PasswordUpdated);
                    }
                    else
                    {
                        return apiResponse.HandleException(SystemMessage.InvalidInput);
                    }
                }
                else
                {
                    return apiResponse.HandleException(SystemMessage.InvalidInput);
                }
            }
            catch (Exception ex)
            {
                return apiResponse.HandleException(ex.Message);
            }
        }

        [HttpPost]
        [Route("ResetPassword")]
        [Authorize]
        public async Task<ApiResponse<string>> ResetPassword(ResetPasswordDto model)
        {
            var apiResponse = new ApiResponse<string>();
            try
            {
                var authorizationHeader = Request.Headers[HeaderNames.Authorization].FirstOrDefault();
                var token = "";
                if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
                {
                    token = authorizationHeader.Split(' ')[1];
                }
                var jwtSecurityToken = JwtService.ReadSecurityToken(token);
                var userEmail = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Email)?.Value;

                if (!string.IsNullOrEmpty(userEmail))
                {
                    var user = await _userRepository.GetFirstAsync(u => u.Email == userEmail);

                    if (user != null && user.UserId > 0)
                    {
                        if (user.Password != EncryptionDecryption.GetEncrypt(model.OldPassword))
                        {
                            return apiResponse.HandleBadRequest(SystemMessage.IncorrectOldPassword);
                        }
                        else
                        {
                            if (user.Password == EncryptionDecryption.GetEncrypt(model.NewPassword))
                            {
                                return apiResponse.HandleBadRequest(SystemMessage.OldNewPasswordsSame);
                            }
                            else
                            {
                                user.Password = EncryptionDecryption.GetEncrypt(model.NewPassword);
                                user.ModifiedDate = DateTime.Now;
                                await _userRepository.UpdateAsync(user);
                                await _unitOfWork.SaveAsync();
                                return apiResponse.HandleResponse(SystemMessage.PasswordUpdated);
                            }
                        }
                    }
                    else
                    {
                        return apiResponse.HandleBadRequest(SystemMessage.ErrorOnUpdatingPassword);
                    }
                }
                else
                {
                    return apiResponse.HandleBadRequest(SystemMessage.ErrorOnUpdatingPassword);
                }
            }
            catch (Exception ex)
            {
                return apiResponse.HandleException(ex.Message);
            }

        }
    }
}
