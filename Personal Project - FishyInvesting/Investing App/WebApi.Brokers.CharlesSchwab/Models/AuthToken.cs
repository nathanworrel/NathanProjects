using Microsoft.EntityFrameworkCore;

namespace WebApi.Template.Models;

public class AuthToken {
    public int Id { get; set; }
    public int UserId { get; set; }
    public string AccessToken { get; set; }
    public DateTime AccessTokenExpiration { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
    public string AuthorizationCode { get; set; }
    public string AppKey { get; set; }
    public string AppSecret { get; set; }

    public AuthToken(int userId, string accessToken, DateTime accessTokenExpiration, string refreshToken,
        DateTime refreshTokenExpiration, string authorizationCode, string appKey, string appSecret)
    {
        UserId = userId;
        AccessToken = accessToken;
        AccessTokenExpiration = accessTokenExpiration;
        RefreshToken = refreshToken;
        RefreshTokenExpiration = refreshTokenExpiration;
        AuthorizationCode = authorizationCode;
        AppSecret = appSecret;
        AppKey = appKey;
    }
}

public class AccountResponse
{
    public string AccountNumber { get; set; }
    public string HashValue { get; set; }
}