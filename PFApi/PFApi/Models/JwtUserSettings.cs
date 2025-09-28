public class JwtUserSettings
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public int RefreshTokenExpiresMinutes { get; set; }
    public int TokenValidMinutes { get; set; }
}

public class RefreshToken
{
    public int Id { get; set; }
    public long UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public bool Revoked { get; set; } = false;
}
