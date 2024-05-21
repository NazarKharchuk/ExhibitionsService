using ExhibitionsService.BLL.Infrastructure.Exceptions;
using ExhibitionsService.PL.Models.HelperModel;
using Stripe;
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
            int statusCode;
            switch (exception)
            {
                case EntityNotFoundException:
                    {
                        statusCode = (int)HttpStatusCode.NotFound;
                        break;
                    }
                case ValidationException:
                    {
                        statusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    }
                case InsufficientPermissionsException:
                    {
                        statusCode = (int)HttpStatusCode.Forbidden;
                        break;
                    }
                case ArgumentException:
                    {
                        statusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    }
                case StripeException:
                    {
                        statusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    }
                default:
                    {
                        statusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                    }
            }
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            var exceptionResult = JsonSerializer.Serialize(new ResponseModel<Object> {
                Successfully = false,
                Message = exception.Message,
                Code = statusCode,
            });
            return context.Response.WriteAsync(exceptionResult);
        }
    }
}
