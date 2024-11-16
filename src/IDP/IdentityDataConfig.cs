public class IdentityDataConfig
{
    public required string BackendClientId { get; set; }
    public required string BackendClientSecret { get; set; }
    public required string WebPortalClientId { get; set; }
    public required string WebPortalClientSecret { get; set; }
    public required string PortalClientRedirectUrl { get; set; }
    public required string PortalClientPostLogoutUrl { get; set; }
    public required string DemoUserName { get; set; }
    public required string DemoUserPassword { get; set; }
}
