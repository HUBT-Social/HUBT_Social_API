using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBT_Social_API.Features.Auth.Dtos.Reponse;
using HUBT_Social_API.Features.Auth.Models;
using HUBT_Social_API.Features.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Auth.Services.Child;

public class TokenService : ITokenService
{
    private readonly JwtSetting _jwtSetting;
    private readonly IMongoCollection<UserToken> _refreshToken;
    private readonly UserManager<AUser> _userManager;

    public TokenService(
        UserManager<AUser> userManager,
        IOptions<JwtSetting> jwtSettings,
        IMongoCollection<UserToken> refreshTokenCollection
    )
    {
        _userManager = userManager;
        _jwtSetting = jwtSettings.Value;
        _refreshToken = refreshTokenCollection;
    }

    // Tạo JWT token và handle Refresh Token
    public async Task<TokenResponse> GenerateTokenAsync(AUser user)
    {
        List<Claim> claims = new();

        claims.AddRange(await _userManager.GetClaimsAsync(user));

        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));
        claims.AddRange(roleClaims);

        // Tạo JWT token
        var token = GenerateAccessToken(claims);
        var refreshToken = GenerateRefreshToken(claims);

        // Xử lý Refresh Token: Cập nhật hoặc tạo mới
        await HandleRefreshTokenAsync(user, token, refreshToken);

        return new TokenResponse
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            ExpiresIn = _jwtSetting.TokenExpirationInMinutes,
            TokenType = "bearer"
        };
    }


    public async Task<UserResponse> GetCurrentUser(string accessToken)
    {
        var decodeValue = await ValidateTokens(accessToken);
        if (!decodeValue.Success)
        {
            return new UserResponse 
            { 
                Success = false, 
                Message = decodeValue.Message 
            };
        }

        var userIdClaim = decodeValue.ClaimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return new UserResponse 
            { 
                Success = false, 
                Message = LocalValue.Get(KeyStore.UserNotFound) 
            };
        }

        // Kiểm tra người dùng bằng userId
        AUser? user = await _userManager.FindByIdAsync(userIdClaim);
        return user == null 
            ? new UserResponse { Success = false, Message = LocalValue.Get(KeyStore.UserNotFound) }
            : new UserResponse { Success = true, User = user };
    }

    private DecodeTokenResponse ValidateToken(string accessToken, string secretKey)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        byte[] tokenKey = Encoding.UTF8.GetBytes(secretKey);
        try
        {
            ClaimsPrincipal principal = tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                ValidIssuer = _jwtSetting.Issuer,
                ValidAudience = _jwtSetting.Audience
            }, out SecurityToken? securityToken);

            if (securityToken is JwtSecurityToken token && token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            {
                if (token.ValidTo < DateTime.UtcNow)
                    return new DecodeTokenResponse { Success = false, Message = "Token is expired" };
                return new DecodeTokenResponse { Success = true, ClaimsPrincipal = principal , Message = LocalValue.Get(KeyStore.TokenValid)};
            }

            return new DecodeTokenResponse { Success = false, Message = "Token is not match our Algorithms" };
        }
        catch (Exception ex)
        {
            return new DecodeTokenResponse { Success = false, Message = ex.Message };
        }
    }
    public async Task<DecodeTokenResponse> ValidateTokens(string accessToken)
    {
        DecodeTokenResponse accessTokenResponse = ValidateToken(accessToken, _jwtSetting.SecretKey);
        if (accessTokenResponse.Success)
        {
            return accessTokenResponse; 
        }
        UserToken userToken = await _refreshToken.Find(t => t.AccessToken == accessToken).FirstOrDefaultAsync();

        DecodeTokenResponse refreshTokenResponse = ValidateToken(userToken.RefreshTo, _jwtSetting.RefreshSecretKey);
        if (refreshTokenResponse.Success)
        {
            AUser? user = await _userManager.FindByIdAsync(userToken.UserId);
            if (user != null)
            {
                await GenerateTokenAsync(user);
            }
        }

        return refreshTokenResponse;
    }

    private async Task HandleRefreshTokenAsync(AUser user, string accessToken, string refreshToken)
    {
        var existingRefreshToken = await _refreshToken.Find(t => t.UserId == user.Id.ToString()).FirstOrDefaultAsync();

        if (existingRefreshToken == null)
        {
            await _refreshToken.InsertOneAsync(new UserToken
                { 
                AccessToken = accessToken,
                RefreshTo = refreshToken,
                UserId = user.Id.ToString(),
                ExpireTime = DateTime.UtcNow.AddDays(_jwtSetting.RefreshTokenExpirationInDays)
            });
        }
        else
        {
            var update = Builders<UserToken>.Update.Set(t => t.AccessToken, accessToken)
                .Set(t => t.RefreshTo, refreshToken)
                .Set(t=> t.ExpireTime, DateTime.UtcNow.AddDays(_jwtSetting.RefreshTokenExpirationInDays));
            await _refreshToken.UpdateOneAsync(t => t.UserId == existingRefreshToken.UserId, update);
        }

    }

    // Tạo JWT Token
    private string GenerateToken(IEnumerable<Claim> claims, string secretKey, Func<DateTime> expiration)
    {
        // Kiểm tra giá trị của SecretKey
        if (string.IsNullOrEmpty(secretKey)) throw new ArgumentException("SecretKey must not be null or empty.");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration(),
            SigningCredentials = credentials,
            Issuer = _jwtSetting.Issuer,
            Audience = _jwtSetting.Audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        return GenerateToken(
            claims,
            _jwtSetting.SecretKey,
            () => DateTime.UtcNow.AddMinutes(_jwtSetting.TokenExpirationInMinutes)
        );
    }

    private string GenerateRefreshToken(IEnumerable<Claim> claims)
    {
        return GenerateToken(
            claims,
            _jwtSetting.RefreshSecretKey,
            () => DateTime.UtcNow.AddDays(_jwtSetting.RefreshTokenExpirationInDays)
        );
    }


}