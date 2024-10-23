using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace BlazorAppWithCustomAuth.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        public CustomAuthStateProvider
        (
            ProtectedSessionStorage sessionStorage
        )
        {
            this._sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {


                var userSessionStorageResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
                var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;
                if (userSession == null)
                    return await Task.FromResult(new AuthenticationState(_anonymous));

                var claimsPrincipal = new ClaimsPrincipal
                    (
                new ClaimsIdentity(new List<Claim>
                        {
                        new(ClaimTypes.Name,userSession.UserName),
                        new(ClaimTypes.Role,userSession.Role),
                        }, "CustomAuth")
                    );
                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
        }

        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            ClaimsPrincipal claimsPrincipal;
            if (userSession != null)
            {
                await _sessionStorage.SetAsync("UserSession", userSession);
                claimsPrincipal = new ClaimsPrincipal
                    (
             new ClaimsIdentity(new List<Claim>
                     {
                        new(ClaimTypes.Name,userSession.UserName),
                        new(ClaimTypes.Role,userSession.Role),
                     }, "CustomAuth")
                );
            }
            else
            {
                await _sessionStorage.DeleteAsync("UserSession");
                claimsPrincipal = _anonymous;
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }
}
