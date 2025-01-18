using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Services.Bases;
using Microsoft.Extensions.Configuration;

namespace FPT.TeamMatching.Services;

public class UserService : BaseService<User>, IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IMapper mapper,
        IUnitOfWork unitOfWork)
        : base(mapper, unitOfWork)
    {
        _userRepository = _unitOfWork.UserRepository;
    }
    // private readonly string _clientId;
    //
    // private readonly IConfiguration _configuration;
    //
    // // private readonly IRefreshTokenRepository _userRefreshTokenRepository;
    // private readonly int _expirationMinutes;
    // private readonly Dictionary<string, DateTime> _expiryStorage = new();
    // private readonly Dictionary<string, string> _otpStorage = new();
    // private readonly IUserRepository _userRepository;
    //
    // public UserService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration)
    //     : base(mapper, unitOfWork)
    // {
    //     _userRepository = _unitOfWork.UserRepository;
    //     // _userRefreshTokenRepository = _unitOfWork.UserRefreshTokenRepository;
    //     _configuration = configuration;
    //     _expirationMinutes = int.Parse(_configuration["TokenSetting:AccessTokenExpiryMinutes"] ?? "30");
    // }

    // private string GenerateSecretKey(int length)
    // {
    //     byte[] secretKey = KeyGeneration.GenerateRandomKey(length);
    //     return Base32Encoding.ToString(secretKey);
    // }
    //
    // public async Task<BusinessResult> UpdatePassword(UserPasswordCommand userPasswordCommand)
    // {
    //     try
    //     {
    //         var userCurrent = GetUser();
    //         if (userCurrent == null) return HandlerFail("Please, login again.");
    //         userCurrent.Password = userPasswordCommand.Password;
    //         InitializeBaseEntityForUpdate(userCurrent);
    //         _userRepository.Update(userCurrent);
    //         var isSave = await _unitOfWork.SaveChanges();
    //
    //         if (!isSave)
    //         {
    //             return HandlerFail(Const.FAIL_SAVE_MSG);
    //         }
    //
    //         return new ResponseBuilder()
    //             .WithStatus(Const.SUCCESS_CODE)
    //             .WithMessage(Const.SUCCESS_READ_MSG)
    //             .Build();
    //     }
    //     catch (Exception ex)
    //     {
    //         return HandlerError(ex.Message);
    //     }
    // }
    //
    // public async Task<BusinessResult> Create(UserCreateCommand createCommand)
    // {
    //     try
    //     {
    //         var entity = await CreateOrUpdateEntity(createCommand);
    //         var userResult = _mapper.Map<UserResult>(entity);
    //
    //         if (userResult == null)
    //         {
    //             return HandlerFail(Const.FAIL_SAVE_MSG);
    //         }
    //
    //         return new ResponseBuilder<UserResult>()
    //             .WithData(userResult)
    //             .WithStatus(Const.SUCCESS_CODE)
    //             .WithMessage(Const.SUCCESS_READ_MSG)
    //             .Build();
    //     }
    //     catch (Exception ex)
    //     {
    //         return HandlerError(ex.Message);
    //     }
    // }
    //
    // public async Task<BusinessResult> Update(UserUpdateCommand updateCommand)
    // {
    //     try
    //     {
    //         var entity = await CreateOrUpdateEntity(updateCommand);
    //         var userResult = _mapper.Map<UserResult>(entity);
    //
    //         if (userResult == null)
    //         {
    //             return HandlerFail(Const.FAIL_SAVE_MSG);
    //         }
    //
    //         return new ResponseBuilder<UserResult>()
    //             .WithData(userResult)
    //             .WithStatus(Const.SUCCESS_CODE)
    //             .WithMessage(Const.SUCCESS_READ_MSG)
    //             .Build();
    //     }
    //     catch (Exception ex)
    //     {
    //         return HandlerError(ex.Message);
    //     }
    // }
    //
    // #region Business
    //
    // public BusinessResult SendEmail(string email)
    // {
    //     try
    //     {
    //         string secret = GenerateSecretKey(10); // Bạn có thể chọn độ dài hợp lý, ví dụ: 10
    //         string otp = GenerateOTP(secret); // Tạo OTP
    //
    //         var fromAddress = new MailAddress("sonnh1106.se@gmail.com");
    //         var toAddress = new MailAddress(email);
    //         const string frompass = "lxnx wdda jepd cxcy"; // Bảo mật tốt hơn là lưu trong biến môi trường
    //         const string subject = "OTP code";
    //
    //         var smtp = new SmtpClient
    //         {
    //             Host = "smtp.gmail.com",
    //             Port = 587,
    //             EnableSsl = true,
    //             DeliveryMethod = SmtpDeliveryMethod.Network,
    //             UseDefaultCredentials = false,
    //             Credentials = new NetworkCredential(fromAddress.Address, frompass),
    //             Timeout = 20000
    //         };
    //
    //         using (var message = new MailMessage(fromAddress, toAddress)
    //                {
    //                    Subject = subject,
    //                    Body = otp,
    //                    IsBodyHtml = false,
    //                })
    //         {
    //             smtp.Send(message);
    //             _otpStorage[email] = otp; // Lưu trữ OTP cho email
    //             _expiryStorage[email] = DateTime.UtcNow.AddMinutes(5); // OTP hết hạn sau 5 phút
    //             return new ResponseBuilder()
    //                 .WithStatus(Const.SUCCESS_CODE)
    //                 .WithMessage(Const.SUCCESS_READ_MSG)
    //                 .Build();
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         return HandlerError(ex.Message);
    //     }
    // }
    //
    // private string GenerateOTP(string secret)
    // {
    //     var key = Base32Encoding.ToBytes(secret);
    //     var totp = new Totp(key);
    //     return totp.ComputeTotp(); // Tạo OTP
    // }
    //
    // public BusinessResult ValidateOtp(string email, string otpInput)
    // {
    //     if (_otpStorage.TryGetValue(email, out string storedOtp) &&
    //         _expiryStorage.TryGetValue(email, out DateTime expiry))
    //     {
    //         if (expiry > DateTime.UtcNow && storedOtp == otpInput)
    //         {
    //             return HandlerFail("OTP validation failed");
    //         }
    //     }
    //
    //     return new ResponseBuilder()
    //         .WithStatus(Const.SUCCESS_CODE)
    //         .WithMessage(Const.SUCCESS_READ_MSG)
    //         .Build();
    // }
    //
    //
    // public async Task<BusinessResult> GetByUsername(string username)
    // {
    //     var user = await _userRepository.GetByUsername(username);
    //
    //     var userResult = _mapper.Map<UserResult>(user);
    //
    //     if (userResult == null)
    //     {
    //         return HandlerNotFound("User not found");
    //     }
    //
    //     return new ResponseBuilder<UserResult>()
    //         .WithData(userResult)
    //         .WithStatus(Const.SUCCESS_CODE)
    //         .WithMessage(Const.SUCCESS_READ_MSG)
    //         .Build();
    // }
    //
    // public BusinessResult DecodeToken(string token)
    // {
    //     var handler = new JwtSecurityTokenHandler();
    //
    //     // Kiểm tra nếu token không hợp lệ
    //     if (!handler.CanReadToken(token))
    //     {
    //         throw new ArgumentException("Token không hợp lệ", nameof(token));
    //     }
    //
    //     // Giải mã token
    //     var jwtToken = handler.ReadJwtToken(token);
    //
    //     // Truy xuất các thông tin từ token
    //     var id = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
    //     var name = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
    //     var role = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Role).Value;
    //     var exp = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Expiration).Value;
    //
    //
    //     // Tạo đối tượng DecodedToken
    //     var decodedToken = new DecodedToken
    //     {
    //         Id = id,
    //         Name = name,
    //         Role = role,
    //         Exp = long.Parse(exp),
    //     };
    //
    //     return new ResponseBuilder<DecodedToken>()
    //         .WithData(decodedToken)
    //         .WithStatus(Const.SUCCESS_CODE)
    //         .WithMessage("Decoded Token")
    //         .Build();
    // }
    //
    // private (string token, string expiration) CreateToken(UserResult user)
    // {
    //     var claims = new List<Claim>
    //     {
    //         new Claim("Id", user.Id.ToString()),
    //         new Claim("Role", user.Role.ToString()),
    //         new Claim("Expiration",
    //             new DateTimeOffset(DateTime.Now.AddMinutes(_expirationMinutes)).ToUnixTimeSeconds().ToString())
    //     };
    //
    //     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
    //         _configuration.GetSection("AppSettings:Token").Value!));
    //
    //     var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
    //
    //
    //     var token = new JwtSecurityToken(
    //         claims: claims,
    //         expires: DateTime.Now.AddMinutes(_expirationMinutes),
    //         signingCredentials: creds
    //     );
    //
    //     var jwt = new JwtSecurityTokenHandler().WriteToken(token);
    //
    //     return (jwt, DateTime.Now.AddMinutes(_expirationMinutes).ToString("o"));
    // }
    //
    // private string GenerateRefreshToken()
    // {
    //     var randomNumber = new byte[32];
    //     using (var rng = RandomNumberGenerator.Create())
    //     {
    //         rng.GetBytes(randomNumber);
    //     }
    //
    //     return Convert.ToBase64String(randomNumber);
    // }
    //
    // public async Task<BusinessResult> AddUser(UserCreateCommand user)
    // {
    //     var username = await _userRepository.FindUsernameOrEmail(user.Username);
    //     return username switch
    //     {
    //         null => await base.CreateOrUpdate<UserResult>(user),
    //         _ => HandlerFail("The account is already registered.")
    //     };
    // }
    //
    // public async Task<BusinessResult> GetByUsernameOrEmail(string key)
    // {
    //     var user = await _userRepository.FindUsernameOrEmail(key);
    //
    //     var userResult = _mapper.Map<UserLoginResult>(user);
    //
    //     return userResult switch
    //     {
    //         null => HandlerNotFound(),
    //         _ => new ResponseBuilder()
    //             .WithStatus(Const.SUCCESS_CODE)
    //             .WithMessage(Const.SUCCESS_READ_MSG)
    //             .Build()
    //     };
    // }
    //
    // public async Task<BusinessResult> GetByRefreshToken(UserGetByRefreshTokenQuery request)
    // {
    //     if (request.RefreshToken == null) return HandlerNotFound("Refresh token is null");
    //     var userRefreshToken = await _userRefreshTokenRepository.GetByRefreshTokenAsync(request.RefreshToken);
    //     var userRefreshTokenResult = _mapper.Map<UserRefreshTokenResult>(userRefreshToken);
    //
    //     if (userRefreshTokenResult == null) return HandlerNotFound("Not found refresh token");
    //
    //     return new ResponseBuilder<UserRefreshTokenResult>()
    //         .WithData(userRefreshToken)
    //         .WithStatus(Const.SUCCESS_CODE)
    //         .WithMessage(Const.SUCCESS_LOGIN_MSG)
    //         .Build();
    // }
    //
    // #endregion
    //
    // private async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string token)
    // {
    //     var settings = new GoogleJsonWebSignature.ValidationSettings()
    //     {
    //         Audience = new List<string> { _clientId }
    //     };
    //
    //     GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
    //     return payload;
    // }
    //
    // public async Task<BusinessResult> VerifyGoogleTokenAsync(VerifyGoogleTokenRequest request)
    // {
    //     var payload = await VerifyGoogleToken(request.Token!);
    //
    //     return payload switch
    //     {
    //         null => new ResponseBuilder()
    //             .WithStatus(Const.FAIL_CODE)
    //             .WithMessage("Invalid Google Token")
    //             .Build(),
    //         _ => new ResponseBuilder<GoogleJsonWebSignature.Payload>()
    //             .WithData(payload)
    //             .WithStatus(Const.SUCCESS_CODE)
    //             .WithMessage("Validate Google Token")
    //             .Build(),
    //     };
    // }
    //
    //
    // public async Task<BusinessResult> LoginByGoogleTokenAsync(VerifyGoogleTokenRequest request)
    // {
    //     var response = VerifyGoogleTokenAsync(request).Result;
    //
    //     if (response.Status != Const.SUCCESS_CODE)
    //     {
    //         return response;
    //     }
    //
    //     var payload = response.Data as GoogleJsonWebSignature.Payload;
    //     var user = await _userRepository.GetByEmail(payload.Email);
    //
    //     if (user == null)
    //     {
    //         return new BusinessResult(Const.FAIL_CODE, Const.NOT_FOUND_USER_LOGIN_BY_GOOGLE_MSG);
    //     }
    //
    //     var userResult = _mapper.Map<UserResult>(user);
    //     var (token, expiration) = CreateToken(userResult);
    //     var loginResponse = new LoginResponse(token, expiration);
    //
    //     return new ResponseBuilder<LoginResponse>()
    //         .WithData(loginResponse)
    //         .WithStatus(Const.SUCCESS_CODE)
    //         .WithMessage(Const.SUCCESS_LOGIN_MSG)
    //         .Build();
    // }
    //
    // public async Task<BusinessResult> FindAccountRegisteredByGoogle(VerifyGoogleTokenRequest request)
    // {
    //     var verifyGoogleToken = new VerifyGoogleTokenRequest
    //     {
    //         Token = request.Token
    //     };
    //
    //     var response = VerifyGoogleTokenAsync(verifyGoogleToken).Result;
    //
    //     if (response.Status != Const.SUCCESS_CODE)
    //     {
    //         return response;
    //     }
    //
    //     var payload = response.Data as GoogleJsonWebSignature.Payload;
    //     var user = await _userRepository.GetByEmail(payload.Email);
    //     var userResult = _mapper.Map<UserResult>(user);
    //     if (userResult == null)
    //     {
    //         return new BusinessResult(Const.SUCCESS_CODE, "Email has not registered by google", null);
    //     }
    //
    //     return new BusinessResult(Const.SUCCESS_CODE, "Email has registered by google", userResult);
    // }
    //
    // public async Task<BusinessResult> RegisterByGoogleAsync(UserCreateByGoogleTokenCommand request)
    // {
    //     var verifyGoogleToken = new VerifyGoogleTokenRequest
    //     {
    //         Token = request.Token
    //     };
    //
    //     var response = VerifyGoogleTokenAsync(verifyGoogleToken).Result;
    //
    //     if (response.Status != Const.SUCCESS_CODE)
    //     {
    //         return response;
    //     }
    //
    //     var payload = response.Data as GoogleJsonWebSignature.Payload;
    //     var user = await _userRepository.GetByEmail(payload.Email);
    //
    //     if (user != null)
    //     {
    //         return new BusinessResult(Const.FAIL_CODE, "Email has existed in server");
    //     }
    //
    //     //string base64Image = await GetBase64ImageFromUrl(payload.Picture);
    //     var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
    //     UserCreateCommand _user = new UserCreateCommand
    //     {
    //         Username = payload.Subject,
    //         Email = payload.Email,
    //         Password = passwordHash,
    //         FirstName = payload.GivenName,
    //         LastName = payload.FamilyName,
    //         Role = Role.Customer,
    //         Avatar = payload.Picture
    //     };
    //
    //     var _response = await AddUser(_user);
    //     var _userAdded = _response.Data as User;
    //     var userResult = _mapper.Map<UserResult>(_userAdded);
    //     var (token, expiration) = CreateToken(userResult);
    //     var loginResponse = new LoginResponse(token, expiration);
    //
    //     return new ResponseBuilder<LoginResponse>()
    //         .WithData(loginResponse)
    //         .WithStatus(Const.SUCCESS_CODE)
    //         .WithMessage(Const.SUCCESS_LOGIN_MSG)
    //         .Build();
    // }
}