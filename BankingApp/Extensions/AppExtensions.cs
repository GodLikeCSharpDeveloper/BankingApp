using Microsoft.AspNetCore.Diagnostics;

namespace BankingApp.Extensions
{
    public static class AppExtensions
    {
        public static void ConfigureExceptionHandler(this WebApplication webApplication)
        {
            webApplication.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogError(exception, "Unhandled exception");

                    var result = new
                    {
                        error = "Internal Server Error",
                        details = exception?.Message
                    };

                    await context.Response.WriteAsJsonAsync(result);
                });
            });
        }
    }
}