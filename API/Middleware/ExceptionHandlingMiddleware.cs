using Common.Utils.Exceptions;
using System.Text.Json;

namespace API.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, _logger);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger logger)
        {
            var statusCode = GetStatusCode(ex);
            var message = GetExceptionMessage(ex);

            logger.LogError(ex, "Unhandled exception occurred");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsync(JsonSerializer.Serialize(new { error = message }));
        }

        private static int GetStatusCode(Exception ex)
        {
            return ex switch
            {
                UnauthorizedAccessException => StatusCodes.Status403Forbidden,
                UserNotInHouseholdException => StatusCodes.Status403Forbidden,
                FinancialMonthOfWrongFormatException => StatusCodes.Status400BadRequest,
                MissingOrWrongDataException => StatusCodes.Status400BadRequest,
                HouseholdWithMoreThanTwoUsersNotSupportedException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        private static string GetExceptionMessage(Exception ex)
        {
            return ex switch
            {
                UnauthorizedAccessException => ex.Message,
                UserNotInHouseholdException => "User is not authorized to view transactions for this household.",
                FinancialMonthOfWrongFormatException => "Financial month of wrong format exception",
                MissingOrWrongDataException => "Missing data",
                HouseholdWithMoreThanTwoUsersNotSupportedException => "More than 2 users in the houshold",
                _ => "Something went wrong."
            };
        }
    }

}
