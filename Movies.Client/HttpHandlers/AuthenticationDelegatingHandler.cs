using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Movies.Client.HttpHandlers
{
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {
        //گرفتن درخواست جاری

        private readonly IHttpContextAccessor _httpContextAccessor;
        //ایجاد یا ساختن یک درخواست 

        private readonly IConfiguration _configuration;

        public AuthenticationDelegatingHandler(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        }



        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = string.Empty;

            var currentContext = _httpContextAccessor.HttpContext;
            var expires_at = await currentContext.GetTokenAsync("expires_at");

            if (string.IsNullOrWhiteSpace(expires_at) || (DateTime.Parse(expires_at).AddSeconds(-60).ToUniversalTime() < DateTime.UtcNow))
            {
                accessToken = await RenewTokens();
            }
            else
            {
                accessToken = await _httpContextAccessor
                               .HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            }


            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.SetBearerToken(accessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string> RenewTokens()
        {

            var httpClient = new HttpClient();
            var currenContext = _httpContextAccessor.HttpContext;

            var currentRefreshToken = await currenContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);            


            
            var response = await httpClient.RequestTokenAsync(new RefreshTokenRequest
            {
                Address = _configuration["IDP_EndPoint"],
                ClientId = _configuration["ClientId"],
                ClientSecret = _configuration["ClientSecret"],
                GrantType= "refresh_token",
                RefreshToken = currentRefreshToken

            });

           
            // refresh the tokens

            if (response.IsError)
            {
                throw new Exception("Problem encountered while refreshing tokens.", response.Exception);
            }

            // update the tokens & expiration value
            var updatedTokens = new List<AuthenticationToken>();
            updatedTokens.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.IdToken,
                Value = response.IdentityToken
            });
            updatedTokens.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.AccessToken,
                Value = response.AccessToken
            });
            updatedTokens.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.RefreshToken,
                Value = response.RefreshToken
            });

            var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(response.ExpiresIn);
            updatedTokens.Add(new AuthenticationToken
            {
                Name = "expires_at",
                Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
            });

            // get authenticate result, containing the current principal & properties
            var currentAuthenticateResult = await currenContext.AuthenticateAsync("Cookies");

            // store the updated tokens
            currentAuthenticateResult.Properties.StoreTokens(updatedTokens);

            // sign in
            await currenContext.SignInAsync("Cookies",
             currentAuthenticateResult.Principal, currentAuthenticateResult.Properties);

            // return the new access token
            return response.AccessToken;
        }
    }
}