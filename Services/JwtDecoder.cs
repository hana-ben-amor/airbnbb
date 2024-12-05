using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

public class JwtDecoder
{
    public static void DecodeJwtToken(string token)
    {
        try
        {
            // Create a JwtSecurityTokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Read and parse the token
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Extract the claims from the token
            var claims = jwtToken.Claims.ToList();

            // Print out all claims for debugging
            Console.WriteLine("Claims in the token:");
            foreach (var claim in claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }

            // Extract specific claims
            var userIdClaim = claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var userRoleClaim = claims.FirstOrDefault(c => c.Type == "Role")?.Value;

            // Output specific claims
            Console.WriteLine($"User ID: {userIdClaim}");
            Console.WriteLine($"User Role: {userRoleClaim}");

            // If you need to return the userId as an integer
            if (int.TryParse(userIdClaim, out var userId))
            {
                Console.WriteLine($"Parsed User ID: {userId}");
            }
            else
            {
                Console.WriteLine("Failed to parse User ID.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error decoding token: " + ex.Message);
        }
    }
}
