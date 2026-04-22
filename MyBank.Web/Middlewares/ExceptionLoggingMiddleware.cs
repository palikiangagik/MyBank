using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MyBank.Web.Services.Account;
using System;
using System.Threading.Tasks;

namespace MyBank.Web.Middlewares
{
    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionLoggingMiddleware> _logger;

        public ExceptionLoggingMiddleware(RequestDelegate next,
            ILogger<ExceptionLoggingMiddleware> logger) 
        {
            this._next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unhandled exception occurred at {context.Request.Path}: {ex.Message}");
                throw;
            }
        }
    }
}
