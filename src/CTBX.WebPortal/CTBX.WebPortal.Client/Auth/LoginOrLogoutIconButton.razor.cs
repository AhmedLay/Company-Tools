using Microsoft.AspNetCore.Components;

namespace CTBX.WebPortal.Client.Auth;

public class LoginOrLogoutIconButtonBase : ComponentBase
{
    [Parameter]
    public string CurrentUrl { get; set; } = "";
}
