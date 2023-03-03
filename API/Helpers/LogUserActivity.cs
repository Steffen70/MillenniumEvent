using System;
using System.Threading.Tasks;
using API.Data;
using API.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (resultContext.HttpContext.User.Identity is not { IsAuthenticated: true }) return;

            var dataContext = resultContext.HttpContext.RequestServices.GetService<DataContext>();

            var userId = resultContext.HttpContext.User.GetUserId();
            var user = await dataContext.Users.FindAsync(userId) ?? throw new Exception($"User: \"{userId}\", could not be found!");

            user.LastActive = DateTime.UtcNow;
            await dataContext.SaveChangesAsync();
        }
    }
}