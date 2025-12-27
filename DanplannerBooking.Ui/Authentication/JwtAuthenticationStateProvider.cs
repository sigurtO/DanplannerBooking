using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;

namespace DanplannerBooking.Ui.Authentication
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public JwtAuthenticationStateProvider(HttpClient http, ILocalStorageService localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
                return new AuthenticationState(_anonymous);

            var user = CreateClaimsPrincipalFromJwt(token);

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return new AuthenticationState(user);
        }

        public async Task MarkUserAsAuthenticated(string token)
        {
            await _localStorage.SetItemAsync("authToken", token);

            var user = CreateClaimsPrincipalFromJwt(token);

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(user))
            );
        }

        public async Task MarkUserAsLoggedOut()
        {
            // Fjern token fra localstorage
            await _localStorage.RemoveItemAsync("authToken");

            // Nulstil HttpClient Authorization-header
            _http.DefaultRequestHeaders.Authorization = null;

            // Fortæl hele UI'et at vi nu er anonyme
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(_anonymous))
            );
        }


        private ClaimsPrincipal CreateClaimsPrincipalFromJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var identity = new ClaimsIdentity(jwt.Claims, "jwt");

            return new ClaimsPrincipal(identity);
        }
    }
}
