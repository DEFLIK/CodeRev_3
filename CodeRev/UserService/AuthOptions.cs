using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace UserService
{
    public class AuthOptions
    {
        public const string Issuer = "AuthServer"; // token maker
        public const string Audience = "AuthClient"; // token consumer
        private const string SecretKey = "mysupersecret_secretkey!123";   // key to encrypt; min 128 bits
        public const int Lifetime = 3600; // token lifetime - secs
        
        // public static string Issuer { get; set; } // token maker
        // public static string Audience { get; set; }  // token consumer
        // private static string SecretKey { get; set; }    // key to encrypt; min 128 bits
        // public static int Lifetime { get; set; }  // token lifetime - secs
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        }
    }
}