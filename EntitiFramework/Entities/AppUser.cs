using Microsoft.AspNetCore.Identity;

namespace WebApplicationMVC.EntitiFramework.Entities
{
    public class AppUser : IdentityUser
    {
        // Полное имя для отображения
        public string FullName { get; set; } = string.Empty;

        // Дата рождения из RegisterViewModel
        public DateTime? DateOfBirth { get; set; }

        // Активен ли пользователь
        public bool IsActive { get; set; } = true;
    }
}
