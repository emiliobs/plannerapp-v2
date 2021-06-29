using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

public class JwtAutenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;

    public JwtAutenticationStateProvider(ILocalStorageService localStorage)
    {
        this._localStorage = localStorage;
    }

    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (await _localStorage.ContainKeyAsync("access_token"))
        {
            //The user is logged
            var tokenAsString = await _localStorage.GetItemAsStringAsync("access_token");
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.ReadJwtToken(tokenAsString);
            var identity = new ClaimsIdentity(token.Claims, "Bearer");
            var user = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(user);

            NotifyAuthenticationStateChanged(Task.FromResult(authState));

            return authState;
        }

        return new AuthenticationState(new ClaimsPrincipal());//Emty claims priincipal mean no identity and the user is no logged in
    }
}
