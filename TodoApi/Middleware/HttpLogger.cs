using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace TodoApi.Middleware
{
    public class HttpLogger
    {
        private readonly RequestDelegate _next;

        public HttpLogger(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var request = await FormatRequest(context.Request);

            Console.WriteLine("- Request:\n" + request);

            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();

            context.Response.Body = responseBody;

            await _next(context);

            var response = await FormatResponse(context.Response);

            Console.WriteLine("- Response:\n" + response + "\n");

            await responseBody.CopyToAsync(originalBodyStream);
        }

        private static async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering();

            var body = await ReadBody(request);

            var bodyString = string.IsNullOrEmpty(body) ? "" : $"Body:\t{body}";

            return $"{request.Method} " +
                $"{request.Scheme}" +
                $"{request.Host}{request.Path}{request.QueryString}\n" +
                $"{GetHeaders(request)}"+
                $"{bodyString}";
        }

        private static async Task<string> FormatResponse(HttpResponse response)
        {
            var body = JsonConvert.DeserializeObject(await ReadBody(response)).ToString();

            var bodyString = string.IsNullOrEmpty(body) ? "" : $"\nBody:\n{body}";

            var code = response.StatusCode;

            return $"Status Code: {code} " +
                $"({ReasonPhrases.GetReasonPhrase(code)})\n" +
                $"{GetHeaders(response)}" +
                $"{bodyString}";
        }

        private static async Task<string> ReadBody(dynamic message)
        {
            message.Body.Seek(0, SeekOrigin.Begin);

            string body = await new StreamReader(message.Body).ReadToEndAsync();

            message.Body.Seek(0, SeekOrigin.Begin);

            return body;
        }

        private static string GetHeaders(dynamic request)
        {
            var headers = "";
            foreach (var header in request.Headers)
                headers += $"\t{header.Key}: {header.Value}\n";
            return headers;
        }
    }
}
