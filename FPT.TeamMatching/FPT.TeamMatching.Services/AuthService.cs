using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Requests.Commands.RefreshTokens;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Users;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Users;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Utilities;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FPT.TeamMatching.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly IMapper _mapper;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    protected readonly TokenSetting _tokenSetting;
    protected readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;

    public AuthService(IMapper mapper,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        IUserService userService,
        IOptions<TokenSetting> tokenSetting,
        IRefreshTokenService refreshTokenService
    )
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _httpContextAccessor ??= new HttpContextAccessor();
        _tokenSetting = tokenSetting.Value;
        _userRepository = _unitOfWork.UserRepository;
        _userService = userService;
        _refreshTokenService = refreshTokenService;
        _refreshTokenRepository = _unitOfWork.RefreshTokenRepository;
    }

    public async Task<RSA> GetRSAKeyFromTokenAsync(string token, string kid)
    {
        // Bước 1: Đọc token và check refreshToken entity bằng user Id và kid
        // * vì sao không check bằng token do token ở đây thường là accessToken nên ko theer check 
        // * do là db đang lưu refreshToken
        // * nên sử dụng kid ở trong token do nó được tạo chung bởi rsa
        var userId = GetUserIdFromToken(token);

        var refreshTokenEntity = await _refreshTokenRepository.GetByUserIdAndKeyIdAsync(Guid.Parse(userId), kid);
        if (refreshTokenEntity == null) throw new Exception("RefreshToken entity not found");

        // Bước 2: Xác thực chữ ký JWT bằng public key
        var isValid = ValidateJwtSignature(refreshTokenEntity.Token, refreshTokenEntity.PublicKey);
        if (!isValid) throw new Exception("Invalid refresh token signature.");

        // Bước 3: Check ipAddress trùng với db 
        var ipAddress = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                        ?? _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";

        ipAddress = NormalizeIpAddress(ipAddress);

        if (refreshTokenEntity.IpAddress != ipAddress) throw new Exception("Ip not matched");

        return LoadRSAFromXml(refreshTokenEntity.PublicKey);
    }


    public async Task<BusinessResult> Login(AuthQuery query)
    {
        // check role
        // var semesterUpComing = await _unitOfWork.SemesterRepository.GetUpComingSemester();
                                                                          // if (semesterUpComing == null)
                                                                          //     return new ResponseBuilder()
                                                                          //         .WithStatus(Const.NOT_FOUND_CODE)
                                                                          //         .WithMessage("Hệ thống chưa cập nhật kì mới.");

        var user = await _userRepository.GetUserByUsernameOrEmail(query.Account);

        if (user == null)
            return new ResponseBuilder()
                .WithStatus(Const.NOT_FOUND_CODE)
                .WithMessage("Email không tìm thấy.");

        var hasRole = user.UserXRoles.Any(m => ((m.SemesterId != null)));
        if (!hasRole)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("Tài khoản chưa được phân quyền. Vui lòng liên hệ quản trị viên để được hỗ trợ.");
        }
        
        if (user.Department != query.Department)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("Tài khoản không có quyền truy cập ở khu vực này");
        }

        if (user.Password == null)
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("Mật khẩu của tài khoản chưa thiết lập mật khẩu. Vui lòng đăng nhập bằng google!");

        if (!BCrypt.Net.BCrypt.Verify(query.Password, user.Password))
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("Mật khẩu không đúng");

        var result = _mapper.Map<UserResult>(user);

        // Tạo cặp khóa RSA
        using (var rsa = new RSACryptoServiceProvider(2048))
        {
            try
            {
                // Lấy public key (dạng XML) để lưu vào database
                var publicKey = rsa.ToXmlString(false);

                // 🟢 Tạo kid từ publicKey (hash SHA256)
                var kid = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(publicKey)));

                var currentSemester = await _unitOfWork.SemesterRepository.GetCurrentSemester();
// Tạo access token với private key
                var accessToken = CreateToken(result, rsa, "AccessToken", kid, currentSemester?.Id);

// Tạo refresh token với kid giống access token
                var refreshTokenValue = CreateToken(result, rsa, "RefreshToken", kid, currentSemester?.Id);

// Lưu refresh token và public key vào database
                var refreshTokenCreateCommand = new RefreshTokenCreateCommand
                {
                    UserId = user.Id,
                    Token = refreshTokenValue,
                    PublicKey = publicKey,
                    KeyId = kid // 🟢 Lưu kid vào database
                };

                var res = _refreshTokenService.CreateOrUpdate<RefreshTokenResult>(refreshTokenCreateCommand).Result;

                if (res.Status != 1)
                    return new ResponseBuilder()
                            .WithStatus(Const.FAIL_CODE)
                            .WithMessage(Const.FAIL_SAVE_MSG)
                        ;

                var refreshToken = res.Data as RefreshTokenResult;

                if (refreshToken == null)
                    return new ResponseBuilder()
                            .WithStatus(Const.FAIL_CODE)
                            .WithMessage("Error while saving refresh token.")
                        ;

                SaveHttpOnlyCookie(accessToken, refreshToken.Token!);

                return new ResponseBuilder()
                        .WithStatus(Const.SUCCESS_CODE)
                        .WithMessage("Đăng nhập thành công")
                    ;
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }
    }

    public async Task<BusinessResult> GetUserByCookie()
    {
        BusinessResult? businessResult = null;

        #region Check refresh token

        var refreshToken = _httpContextAccessor.HttpContext.Request.Cookies["refreshToken"];
        businessResult = ValidateRefreshTokenIpAdMatch(refreshToken);
        if (businessResult.Status != 1) return businessResult;

        #endregion

        #region CheckAccessToken

        var accessToken = _httpContextAccessor.HttpContext.Request.Cookies["accessToken"];
        if (accessToken != null)
        {
            var br = await GetUserByClaims();

            return br;
        }

        #endregion

        #region SaveRefreshToken

        businessResult = await RefreshToken(new UserRefreshTokenCommand
        {
            RefreshToken = refreshToken
        });

        if (businessResult.Status != 1) return businessResult;

        #endregion

        #region CheckRefreshToken is valid => return user

        var tokenResult = businessResult.Data as TokenResult;
        businessResult = await GetUserByToken(tokenResult.AccessToken);

        return businessResult;

        #endregion
    }

    public async Task<BusinessResult> RefreshToken(UserRefreshTokenCommand request)
    {
        // Bước 1: Lấy thông tin refresh token từ database
        try
        {
            var refreshTokenEntity = await _refreshTokenRepository.GetByRefreshTokenAsync(request.RefreshToken!);
            if (refreshTokenEntity == null)
                return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage("Refresh token not found.")
                    ;

            // Bước 2: Xác thực chữ ký JWT bằng public key
            var isValid = ValidateJwtSignature(request.RefreshToken, refreshTokenEntity.PublicKey);
            if (!isValid)
                return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Invalid refresh token signature.")
                    ;

            // Bước 3: Kiểm tra IP của request có khớp với IP đã lưu không
            var businessResult = _refreshTokenService.ValidateRefreshTokenIpMatch();

            if (businessResult.Status != 1)
                return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("IP address mismatch.")
                    ;

            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    var newPublicKey = rsa.ToXmlString(false);

                    var user = await _userRepository.GetById(refreshTokenEntity.UserId!.Value);
                    var userResult = _mapper.Map<UserResult>(user);
                    var kid = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(newPublicKey)));

                    var currentSemester = await _unitOfWork.SemesterRepository.GetCurrentSemester();
                    var newAccessToken = CreateToken(userResult, rsa, "AccessToken", kid, currentSemester?.Id);
                    var newRefreshToken = CreateToken(userResult, rsa, "RefreshToken", kid, currentSemester?.Id);

                    refreshTokenEntity.Token = newRefreshToken;
                    refreshTokenEntity.PublicKey = newPublicKey;
                    refreshTokenEntity.KeyId = kid;
                    refreshTokenEntity.Expiry = DateTime.UtcNow.AddDays(_tokenSetting.RefreshTokenExpiryDays);
                    _refreshTokenRepository.Update(refreshTokenEntity);
                    var isSaveChanges = await _unitOfWork.SaveChanges();
                    if (!isSaveChanges)
                        return new ResponseBuilder()
                                .WithStatus(Const.FAIL_CODE)
                                .WithMessage("Refresh token validation failed when saving changes.")
                            ;

                    SaveHttpOnlyCookie(newAccessToken, newRefreshToken);

                    var tokenResult = new TokenResult
                    {
                        AccessToken = newAccessToken,
                        RefreshToken = newRefreshToken
                    };

                    return new ResponseBuilder()
                        .WithData(tokenResult)
                        .WithStatus(Const.SUCCESS_CODE)
                        .WithMessage("Token refreshed successfully.");
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }
        catch (Exception ex)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(ex.Message);
        }
    }

    public async Task<BusinessResult> Logout(UserLogoutCommand userLogoutCommand)
    {
        try
        {
            var userRefreshToken = await _refreshTokenRepository
                .GetByRefreshTokenAsync(userLogoutCommand.RefreshToken ?? string.Empty);
            if (userRefreshToken == null)
                return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage("You are not logged in, please log in to continue.")
                    ;
            _refreshTokenRepository.DeletePermanently(userRefreshToken);

            var isSaved = await _unitOfWork.SaveChanges();
            if (!isSaved) throw new Exception();

            _httpContextAccessor.HttpContext.Response.Cookies.Delete("accessToken");
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("refreshToken");

            return new ResponseBuilder()
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage("The account has been logged out.");
        }
        catch (Exception e)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(e.Message);
        }
    }


    #region login google

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

    private async Task<Userinfo> VerifyGoogleAccessToken(string accessToken)
    {
        var oauthService = new Oauth2Service(new BaseClientService.Initializer
        {
            HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken),
            // ApplicationName = "main"
        });

        // Lấy thông tin user từ Google
        Userinfo userInfo = await oauthService.Userinfo.Get().ExecuteAsync();

        return userInfo; // Trả về object có sẵn từ thư viện
    }


    public async Task<BusinessResult> LoginByGoogleTokenAsync(AuthByGoogleTokenQuery request)
    {
        var payload = await VerifyGoogleAccessToken(request.Token!);
        if (payload == null)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage("Error token google.");
        }

        var user = await _userRepository.GetByEmail(payload.Email);
        var result = _mapper.Map<UserResult>(user);
        if (result == null)
        {
            // register
            UserCreateCommand _userCreateCommand = new UserCreateCommand
            {
                Username = payload.Id,
                Email = payload.Email,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                Avatar = payload.Picture,
                Department = request.Department,
            };
            var res = await _userService.Create(_userCreateCommand);

            if (res.Status != 1)
            {
                return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Error while saving refresh token.")
                    ;
            }

            result = res.Data as UserResult;
        }
        else
        {
            if (result.Department != request.Department)
            {
                return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage("Your account does not have access to the system")
                    ;
            }
        }

        using (var rsa = new RSACryptoServiceProvider(2048))
        {
            try
            {
                var publicKey = rsa.ToXmlString(false);

                var kid = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(publicKey)));

                var currentSemester = await _unitOfWork.SemesterRepository.GetCurrentSemester();
                var accessToken = CreateToken(result, rsa, "AccessToken", kid, currentSemester?.Id);

                var refreshTokenValue = CreateToken(result, rsa, "RefreshToken", kid, currentSemester?.Id);

                var refreshTokenCreateCommand = new RefreshTokenCreateCommand
                {
                    UserId = result.Id,
                    Token = refreshTokenValue,
                    PublicKey = publicKey,
                    KeyId = kid
                };

                var res = _refreshTokenService.CreateOrUpdate<RefreshTokenResult>(refreshTokenCreateCommand).Result;

                if (res.Status != 1)
                    return new ResponseBuilder()
                            .WithStatus(Const.FAIL_CODE)
                            .WithMessage(Const.FAIL_SAVE_MSG)
                        ;

                var refreshToken = res.Data as RefreshTokenResult;

                if (refreshToken == null)
                    return new ResponseBuilder()
                            .WithStatus(Const.FAIL_CODE)
                            .WithMessage("Error while saving refresh token.")
                        ;

                SaveHttpOnlyCookie(accessToken, refreshToken.Token!);

                return new ResponseBuilder()
                        .WithStatus(Const.SUCCESS_CODE)
                        .WithMessage("Login successful.")
                    ;
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }
    }

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
    //         ;
    // }

    #endregion

    private string NormalizeIpAddress(string ipAddress)
    {
        if (ipAddress.Contains(",")) ipAddress = ipAddress.Split(',')[0].Trim();

        if (IPAddress.TryParse(ipAddress, out var ip))
        {
            if (ip.IsIPv4MappedToIPv6) return ip.MapToIPv4().ToString();

            // Chuyển loopback IPv6 (::1) về loopback IPv4 (127.0.0.1)
            if (IPAddress.IPv6Loopback.Equals(ip)) return IPAddress.Loopback.ToString(); // Trả về 127.0.0.1
        }

        return ipAddress;
    }

    public string GetUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value; // "sub" thường là userId
    }

    private string GetKidFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken.Header.Kid; // Lấy Key ID (kid)
    }

    private RSA LoadRSAFromXml(string xmlKey)
    {
        var rsa = RSA.Create();
        rsa.FromXmlString(xmlKey);
        return rsa;
    }

    // private async Task<BusinessResult> SaveRefreshToken(Guid userId, string refreshToken)
    // {
    //     var user = await _userRepository.GetById(userId);
    //     var userResult = _mapper.Map<UserResult>(user);
    //
    //     // Create new access token
    //     var accessToken = CreateToken(userResult);
    //
    //     var accessTokenOptions = new CookieOptions
    //     {
    //         HttpOnly = true,
    //         Secure = true,
    //         SameSite = SameSiteMode.None,
    //         Expires = DateTime.UtcNow.AddMinutes(_tokenSetting.AccessTokenExpiryMinutes),
    //     };
    //
    //     _httpContextAccessor.HttpContext.Response.Cookies.Append("accessToken", accessToken, accessTokenOptions);
    //
    //     var tokenResult = new TokenResult { AccessToken = accessToken, RefreshToken = refreshToken };
    //     return new ResponseBuilder<TokenResult>()
    //         .WithData(tokenResult)
    //         .WithStatus(Const.SUCCESS_CODE)
    //         .WithMessage(Const.SUCCESS_LOGIN_MSG)
    //         ;
    // }

    protected async Task<BusinessResult> GetUserByToken(string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
            return new ResponseBuilder()
                .WithStatus(Const.NOT_FOUND_CODE)
                .WithMessage("No access token provided");

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(accessToken);

        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        if (string.IsNullOrEmpty(userId))
            return new ResponseBuilder()
                .WithStatus(Const.NOT_FOUND_CODE)
                .WithMessage("Error the access token provided");

        var businessResult = await _userService.GetById<UserResult>(Guid.Parse(userId));

        return businessResult;
    }

    private bool ValidateJwtSignature(string token, string publicKey)
    {
        try
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(rsa),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, validationParameters, out _);

            return true;
        }
        catch (SecurityTokenSignatureKeyNotFoundException)
        {
            // Chữ ký không hợp lệ hoặc không tìm thấy key
            Console.WriteLine("Invalid signature or key not found.");
            return false;
        }
        catch (SecurityTokenExpiredException)
        {
            // Token hết hạn
            Console.WriteLine("Token has expired.");
            return false;
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            // Chữ ký không hợp lệ
            Console.WriteLine("Invalid token signature.");
            return false;
        }
        catch (Exception ex)
        {
            // Các lỗi khác
            Console.WriteLine($"Token validation failed: {ex.Message}");
            return false;
        }
    }


    public BusinessResult ValidateRefreshTokenIpAdMatch(string refreshToken)
    {
        if (refreshToken == null)
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_UNAUTHORIZED_CODE)
                .WithMessage(Const.FAIL_UNAUTHORIZED_MSG);

        var businessResult = _refreshTokenService.ValidateRefreshTokenIpMatch();
        return businessResult;
    }

    public async Task<BusinessResult> GetUserByClaims()
    {
        try
        {
            // Lấy thông tin UserId từ Claims
            var userId = GetUserIdFromClaims();
            if (!userId.HasValue)
                return new ResponseBuilder()
                        .WithStatus(Const.NOT_FOUND_CODE)
                        .WithMessage("No user found.")
                    ;

            // Lấy thông tin người dùng từ database
            var userResult = await _userService.GetById<UserResult>(userId.Value);
            return userResult;
        }
        catch (Exception ex)
        {
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(ex.Message);
        }
    }

    private Guid? GetUserIdFromClaims()
    {
        var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("Id")?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return null;

        return Guid.Parse(userIdClaim);
    }

    // private string CreateToken(UserResult user, RSACryptoServiceProvider rsa, string tokenType, string kid)
    // {
    //     var claims = new List<Claim>
    //     {
    //         new("Id", user.Id.ToString()),
    //         new("TokenType", tokenType)
    //     };
    //
    //     if (user.UserXRoles.Count != 0)
    //         foreach (var role in user.UserXRoles)
    //             if (role.Role?.RoleName != null)
    //                 claims.Add(new Claim("Role", role.Role?.RoleName)); 
    //
    //     var key = new RsaSecurityKey(rsa)
    //     {
    //         KeyId = kid
    //     };
    //     var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
    //
    //     var token = new JwtSecurityToken(
    //         claims: claims,
    //         expires: tokenType == "AccessToken"
    //             ? DateTime.Now.AddMinutes(_tokenSetting.AccessTokenExpiryMinutes) // Access token ngắn hạn
    //             : DateTime.Now.AddDays(_tokenSetting.RefreshTokenExpiryDays), // Refresh token dài hạn
    //         signingCredentials: creds
    //     );
    //
    //     return new JwtSecurityTokenHandler().WriteToken(token);
    // }
    //
    private string CreateToken(UserResult user, RSACryptoServiceProvider rsa, string tokenType, string kid,
        Guid? currentSemesterId = null)
    {
        var claims = new List<Claim>
        {
            new("Id", user.Id.ToString()),
            new("TokenType", tokenType)
        };

        if (user.UserXRoles.Count != 0)
        {
            // Lấy role chính (isPrimary = true)
            var primaryRole = user.UserXRoles.FirstOrDefault(x => x.IsPrimary);
            if (primaryRole?.Role?.RoleName != null)
            {
                claims.Add(new Claim("PrimaryRole", primaryRole.Role.RoleName));
            }

            // Lấy TẤT CẢ role phụ theo kỳ học nếu có
            if (currentSemesterId.HasValue)
            {
                var semesterRoles = user.UserXRoles
                    .Where(x => x.SemesterId == currentSemesterId && !x.IsPrimary && x.Role?.RoleName != null)
                    .ToList();

                foreach (var role in semesterRoles)
                {
                    if (!string.IsNullOrEmpty(role.Role?.RoleName))
                    {
                        claims.Add(new Claim("CurrentSemesterRole", role?.Role?.RoleName));
                    }
                }

                // Thêm CurrentSemesterId vào claim (chỉ 1 lần)
                claims.Add(new Claim("CurrentSemesterId", currentSemesterId.Value.ToString()));
            }
        }

        var key = new RsaSecurityKey(rsa)
        {
            KeyId = kid
        };
        var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: tokenType == "AccessToken"
                ? DateTime.Now.AddMinutes(_tokenSetting.AccessTokenExpiryMinutes)
                : DateTime.Now.AddDays(_tokenSetting.RefreshTokenExpiryDays),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private void SaveHttpOnlyCookie(string accessToken, string refreshToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        var accessTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(_tokenSetting.AccessTokenExpiryMinutes)
        };

        var refreshTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(_tokenSetting.RefreshTokenExpiryDays)
        };

        httpContext.Response.Cookies.Append("accessToken", accessToken, accessTokenOptions);
        httpContext.Response.Cookies.Append("refreshToken", refreshToken, refreshTokenOptions);
    }
}