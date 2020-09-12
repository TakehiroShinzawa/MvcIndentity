using MvcIdentity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace MvcIndentity.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetMyPage(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                Claim claim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.MyPage);
                //var claim = from a in claimsIdentity.Claims
                //            where a.Type == CustomClaimTypes.MyPage
                //            select a;

                if (claim != null)
                {
                    return claim.Value;
                }
            }
            return null;
        }
    }
}