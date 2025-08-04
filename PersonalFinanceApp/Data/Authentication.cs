using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace PersonalFinanceApp.Data
{
    public class Authentication : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;

        public Authentication(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var username = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "userName");

            ClaimsPrincipal user;

            if (!string.IsNullOrEmpty(username))
            {
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username)
                }, "serverAuth");

                user = new ClaimsPrincipal(identity);
            }
            else
            {
                user = new ClaimsPrincipal(new ClaimsIdentity());
            }

            return new AuthenticationState(user);
        }

        public async Task MarkUserAsAuthenticated(string username)
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "userName", username);

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }, "serverAuth");

            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "userName");

            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
        }
    }
}
