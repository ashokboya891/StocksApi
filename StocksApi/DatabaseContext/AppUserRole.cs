
using Microsoft.AspNetCore.Identity;
using StocksApi.DatabaseContext;

namespace API.Entities
{
    public class AppUserRole:IdentityUserRole<int>
    {
        public ApplicationUser User { get; set; }
        public ApplicationRole Role{set;get;}

    }
}