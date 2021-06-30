using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dvelop.Remote.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class Dv1HmacSha256SignatureAttribute : TypeFilterAttribute, IAllowAnonymous
    {
        public Dv1HmacSha256SignatureAttribute() : base(typeof(Dv1HmacSha256SignatureFilter))
        {
        }
    }
}