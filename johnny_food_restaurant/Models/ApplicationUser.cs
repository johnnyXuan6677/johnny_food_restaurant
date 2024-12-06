
using Microsoft.AspNetCore.Identity;

namespace johnny_food_restaurant.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Order>? Orders { get; set; }
    }
}
