using System;
using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserName(this ClaimsPrincipal user)
        => user.FindFirst(ClaimTypes.Name)?.Value.ToLower();

        public static int GetUserId(this ClaimsPrincipal user)
        => int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("Can't get UserId from ClaimsPrincipal"));
    }
}