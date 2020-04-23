using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dvelop.Remote.Constraints
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DvSignatureAuthenticationAttribute : TypeFilterAttribute, IAllowAnonymous
    {
        public DvSignatureAuthenticationAttribute() : base(typeof(DvSignatureFilter))
        {
        }
    }
}
