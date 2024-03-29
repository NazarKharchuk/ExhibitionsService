using ExhibitionsService.BLL.Infrastructure.Exceptions;
using System.Net;
using System.Text.Json;

namespace ExhibitionsService.PL.Middlewares
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public GlobalErrorHandlingMiddleware(RequestDelegate _next)
        {
            next = _next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var exceptionResult = JsonSerializer.Serialize(new { error = exception.Message });
            context.Response.ContentType = "application/json";
            switch (exception)
            {
                case EntityNotFoundException:
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    }
                case ValidationException:
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    }
                case ArgumentException:
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    }
                default:
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                    }
            }

            return context.Response.WriteAsync(exceptionResult);
        }
    }
}
