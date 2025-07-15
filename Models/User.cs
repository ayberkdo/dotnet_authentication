using Microsoft.AspNetCore.Identity;

namespace dotnet_authentication.Models
{
    public class User : IdentityUser
    {
        // Sadece gerekli alanlar: Id, UserName, Email, PasswordHash
        // IdentityUser'dan gereksiz property'ler ignore edilecek
    }
}
