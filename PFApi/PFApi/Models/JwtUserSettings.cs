public class JwtUserSettings
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public int RefreshTokenExpiresMinutes { get; set; }
    public int TokenValidMinutes { get; set; }
}