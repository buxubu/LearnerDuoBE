using LearnerDuo.Extentions;
using LearnerDuo.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LearnerDuo.Helper
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next(); 
            
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;
            var userIdClaim = resultContext.HttpContext.User.GetUserId();
            var repo = resultContext.HttpContext.RequestServices.GetService<IUserService>();
            var memberDto = await repo.FindUserId(userIdClaim);
            memberDto.LastActive = DateTime.Now;
            await repo.UpdateUserDb(memberDto);

        }
    }
}
