public class IdentityOptions
{
    public required bool RequireHttpsMetadata { get; set; }
    public required string Audience { get; set; }
    public required string MetadataAddress { get; set; }
    public required string Authority { get; set; }
}
