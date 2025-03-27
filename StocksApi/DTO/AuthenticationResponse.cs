﻿namespace StocksApi.DTO
{
    public class AuthenticationResponse
    {
        public string? PersonName { set; get; }
        public string? Token { set; get; }
        public string? Email { set; get; }
        public DateTime Expire { set; get; }
        public string? RefreshToken { set; get; } = string.Empty;
        public DateTime RefreshTokenExpirationDateTime { get; set; }
    }
}
