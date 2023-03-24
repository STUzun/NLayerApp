using Core;
using Core.DTOs.Errors;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Filters
{
    public class NotFoundFilter<T>:IAsyncActionFilter where T : BaseEntity
    {
        private readonly IService<T> _service;
        public NotFoundFilter(IService<T> service) 
        {
            _service = service;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var idValue = context.ActionArguments.Values.FirstOrDefault();
            if (idValue == null) 
            {
                await next.Invoke();
                return;
            }
            var anyEntity = await _service.AnyAsync(x => x.Id ==(long)idValue);
            if (anyEntity)
            {
                await next.Invoke();
                return;
            }
            var errorViewModel = new ErrorViewModel();
            errorViewModel.Errors.Add($"{typeof(T).Name}({(long)idValue}) not found");
            context.Result = new RedirectToActionResult("Error","Home",errorViewModel);

        }
    }
}
