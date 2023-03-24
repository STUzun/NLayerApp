using Core.DTOs.Custom;
using Microsoft.AspNetCore.Diagnostics;
using Service.Exceptions;
using System.Text.Json;

namespace API.Middlewares
{
    public static class UseCustomExceptionsHandler
    {
        public static void UseCustomExceptions(this IApplicationBuilder app) 
        {
            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    context.Response.ContentType= "application/json";
                    var exceptionFeature=context.Features.Get<IExceptionHandlerFeature>();
                    var statusCode = exceptionFeature.Error switch
                    {
                        ClientSideException => 400,
                        NotFoundException=> 404,
                        _ => 500
                    };
                    context.Response.StatusCode = statusCode;
                    var response = CustomResponseDto<NoContentDto>.Fail(statusCode,exceptionFeature.Error.Message);
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                });
            });
        }
    }
}
