using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Dvelop.Domain.Repositories;
using Microsoft.AspNetCore.Http;

namespace Dvelop.Adapter
{
    public class UserRepository : IUserRepository
    {
        private readonly IHttpContextAccessor _context;

        public UserRepository(IHttpContextAccessor context)
        {
            _context = context;
        }

        public IUser CurrentUser => new User(_context.HttpContext.User);
    }

    public class User : IUser
    {
        public ClaimsPrincipal Principal { get; }
        public string Id => Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)?.Value;
        public string GivenName => Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
        public string SurName => Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
        public string EMail => Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        public string DvPhotoUri => Principal.Claims.FirstOrDefault(c => c.Type == "PhotoUri")?.Value;
        public string DvDisplayName => Principal.Claims.FirstOrDefault(c => c.Type == "DisplayName")?.Value;
        public string DvBearer => Principal.Claims.FirstOrDefault(c => c.Type == "com.dvelop.bearer")?.Value;
        public string DvUserId => Principal.Claims.FirstOrDefault(c => c.Type == "com.dvelop.user.id")?.Value;
        public IEnumerable<string> Roles => Principal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

        public User(ClaimsPrincipal principal)
        {
            Principal = principal;
        }
    }
}
