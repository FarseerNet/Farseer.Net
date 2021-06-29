using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using FS.Core.Exception;
using FS.Extends;
using Microsoft.IdentityModel.Tokens;

namespace FS.Core.Jwt
{
    public class JwtToken
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _securityKey;

        /// <summary>
        /// 生成Token
        /// </summary>
        /// <param name="issuer">发行人</param>
        /// <param name="audience">客户端</param>
        /// <param name="securityKey">秘钥</param>
        public JwtToken(string issuer, string audience, string securityKey)
        {
            this._issuer      = issuer;
            this._audience    = audience;
            this._securityKey = securityKey;
        }

        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="expires">过期时间</param>
        /// <param name="customData">自定义要赋加的数据 </param>
        public TokenInfo Build(DateTime expires, Dictionary<string, string> customData)
        {
            var tokenInfo = new TokenInfo(); //需要返回的口令信息
            try
            {
                // 口令信息
                tokenInfo.Success = true;
                tokenInfo.Token   = BuildToken(expires, customData);
                tokenInfo.Message = "OK";
            }
            catch (System.Exception ex)
            {
                tokenInfo.Success = false;
                tokenInfo.Message = ex.Message;
            }

            return tokenInfo;
        }

        /// <summary>
        /// 生成 token
        /// </summary>
        private string BuildToken(DateTime expires, Dictionary<string, string> customData)
        {
            try
            {
                var now = DateTime.Now;
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityKey)); //获取密钥
                return new JwtSecurityTokenHandler().CreateEncodedJwt(new SecurityTokenDescriptor
                {
                    Issuer             = _issuer,                                                    // 发行人
                    Audience           = _audience,                                                  // 观众
                    Expires            = expires,                                                    // 过期时间
                    IssuedAt           = now,                                                        //  发行时间
                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256), // 加密方式
                    Subject            = new ClaimsIdentity(customData.Select(o => new Claim(o.Key, o.Value)).ToArray())
                });
            }
            catch (System.Exception ex)
            {
                throw new RefuseException($"创建token失败:{ex}");
            }
        }

        /// <summary>
        /// 验证
        /// </summary>
        public bool Validate(string token, out ClaimsPrincipal claimsPrincipal)
        {
            var handler = new JwtSecurityTokenHandler();
            var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityKey)); //获取密钥

            claimsPrincipal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey         = key,

                ValidateIssuer = true,
                ValidIssuer    = _issuer,

                ValidateAudience = true,
                ValidAudience    = _audience,

                ValidateLifetime = true,
                ClockSkew        = TimeSpan.Zero,
            }, out _);
            var issuer      = claimsPrincipal.Claims.FirstOrDefault(o => o.Type == "iss")?.Value;         // 发行人
            var audience    = claimsPrincipal.Claims.FirstOrDefault(o => o.Type == "aud")?.Value;         // 客户端
            var expiresTs   = claimsPrincipal.Claims.FirstOrDefault(o => o.Type == "exp")?.Value;         // 过期时间
            var date        = new DateTime(1970, 1, 1).Add(TimeZoneInfo.Local.BaseUtcOffset).AddSeconds(expiresTs.ConvertType(0L));
            return issuer == _issuer && audience == _audience && date >= DateTime.Now;
        }
    }
}