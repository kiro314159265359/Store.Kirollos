using Domain.Exceptions;
using Shared.ErrorModels;

namespace Store.Kirollos.Api.Middlewares
{
    public class GlobalErrorHandlingMiddleware
    {
        // 1. first we make a class called GlobalErrorHandlingMiddleware in api
        // 2. make a class that represents body of the response (ErrorDetails) in shared
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;
        public GlobalErrorHandlingMiddleware(RequestDelegate next , ILogger<GlobalErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
                if(context.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    await HandlingNotFoundEndpointAsync(context);
                }
            } catch (Exception e)
            {
                // Log exception
                _logger.LogError(e, e.Message);

                await HandlingErrorAsync(context, e);
            }
        }

        private static async Task HandlingErrorAsync(HttpContext context, Exception e)
        {

            // 1. set status code
            //context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            // 2. Set content type
            context.Response.ContentType = "application/json";
            // 3. Response object
            var response = new ErrorDetails()
            {
                ErrorMessage = e.Message
            };

            response.StatusCode = e switch
            {
                NotFoundExecption => StatusCodes.Status404NotFound,
                BadRequestExecption => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError,
            };

            context.Response.StatusCode = response.StatusCode;

            // 4. return response
            await context.Response.WriteAsJsonAsync(response);
        }

        private static async Task HandlingNotFoundEndpointAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            var response = new ErrorDetails()
            {
                StatusCode = StatusCodes.Status404NotFound,
                ErrorMessage = $"End Point {context.Request.Path} is Not Found"
            };
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
