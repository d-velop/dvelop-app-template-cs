using System.Collections.Generic;
using System.Security.Claims;

namespace Dvelop.Domain.Repositories
{
    public interface IUserRepository
    {
        IUser CurrentUser { get; }
    }

    public interface IUser
    {
        ClaimsPrincipal Principal { get; }
        string GivenName { get; }
        string SurName { get; }
        string DvDisplayName { get; }
        string EMail { get; }
        string DvBearer { get; }
        string DvPhotoUri { get; }
        string DvUserId { get; }
        IEnumerable<string> Roles{ get; }
    }
}