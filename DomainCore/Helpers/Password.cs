using System;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace DomainCore.Helpers
{
    public static class Password
    {
        public static string Hash(string password)
        {
            byte[] salt = Encoding.ASCII.GetBytes("OLcbPuUZjKyYtw==");

            Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashed;
        }
    }
}
