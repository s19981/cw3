using cw3.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cw3.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IStudentDbService service)
        {
            httpContext.Request.EnableBuffering();

            if (httpContext.Request != null)
            {
                string sciezka = httpContext.Request.Path;
                string metoda = httpContext.Request.Method.ToString();
                string querystring = httpContext.Request?.QueryString.ToString();
                string bodyStr = "";

                using (StreamReader reader
                 = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                }

                using (System.IO.StreamWriter file = new System.IO.StreamWriter("requestsLog.txt", true))
                {
                    string text = String.Concat("[", DateTime.Now, "] Metoda: ", metoda, ", Sciezka: ", sciezka, ", Cialo: ", bodyStr, ", QueryString: ", querystring);
                    file.WriteLine(text);
                };
            }

            await _next(httpContext);
        }


    }
}
