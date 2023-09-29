using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RestaurantAPI.Middleware
{
    public class RequestTimeMiddleware : IMiddleware
    {
        private Stopwatch _stopwatch = new Stopwatch();
        private ILogger<RequestTimeMiddleware> _logger;

        public RequestTimeMiddleware(ILogger<RequestTimeMiddleware> logger) 
        {
            _logger =  logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _stopwatch.Start();
            await next.Invoke(context);
            _stopwatch.Stop();
            TimeSpan ts = _stopwatch.Elapsed;
            if(ts.Milliseconds > 4)
            {
                var message = $"Request [{context.Request.Method}] at {context.Request.Path} took {ts.Milliseconds} ms";
                _logger.LogInformation(message);
            }
        }
    }
}
