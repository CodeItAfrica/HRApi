﻿namespace HRApi
{
    public class CorsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _allowedOrigins =
        {
            "http://localhost:5173",
            "http://localhost:5174",
            "https://localhost:5173",
            "https://localhost:5174",
            "http://hrms5.newgibsonline.com",
            "https://hrms5.newgibsonline.com",
        };

        public CorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var origin = httpContext.Request.Headers["Origin"].ToString();

            if (_allowedOrigins.Contains(origin) || string.IsNullOrEmpty(origin))
            {
                httpContext.Response.Headers.Add("Access-Control-Allow-Origin", origin ?? "*");
            }

            //httpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");

            httpContext.Response.Headers.Add(
                "Access-Control-Allow-Headers",
                "Content-Type, X-CSRF-Token, X-Requested-With, Accept, Accept-Version, Content-Length, Content-MD5, Date, X-Api-Version, X-File-Name"
            );
            httpContext.Response.Headers.Add(
                "Access-Control-Allow-Methods",
                "POST,GET,PUT,PATCH,DELETE,OPTIONS"
            );
            if (httpContext.Request.Method == "OPTIONS")
            {
                httpContext.Response.StatusCode = 200;
                return;
            }

            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CorsMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorsMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorsMiddleware>();
        }
    }
}
