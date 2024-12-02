using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace CTBX.WebPortal.Client.Auth;

public class LoginOrLogoutIconButtonBase : ComponentBase
{

    [Parameter]
    public string CurrentUrl { get; set; } = "";

    [CascadingParameter]
    public required Task<AuthenticationState> AuthState { get; set; }

    private AuthenticationState? _authenticationState;

    protected override async Task OnInitializedAsync()
    {
        _authenticationState = await AuthState;
    }
    protected string GetLoggedInUsersInitials()
    {

        var email = _authenticationState?.User?.Identity?.Name ?? string.Empty;
        var userName = email.Substring(0,email.IndexOf("@")).Split('.');

        return
            userName.Length >= 2
            ? $"{userName[0][0]}{userName[1][0]}"
            : $"{userName[0][0]}";
    }
}
