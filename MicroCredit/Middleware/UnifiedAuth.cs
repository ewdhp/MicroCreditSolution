#nullable enable
using System;
using System.Threading.Tasks;
using MicroCredit.Services.Auth;
using Microsoft.AspNetCore.Http;

namespace MicroCredit.Middleware
{
    public class UnifiedAuth
    {
        private readonly RequestDelegate _next;
        private readonly TokenValidationService _tokenValidationService;

        public UnifiedAuth
        (
            RequestDelegate next,
            TokenValidationService tokenValidationService)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _tokenValidationService = tokenValidationService ??
            throw new ArgumentNullException(nameof(tokenValidationService));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the request is a WebSocket request
            if (context.WebSockets.IsWebSocketRequest)
            {
                var token = context.Request
                .Headers["Authorization"].ToString();

                if (string.IsNullOrWhiteSpace(token))
                {
                    context.Response.StatusCode = 400; // Bad Request
                    await context.Response.WriteAsync
                    ("Authorization token is missing.");
                    return;
                }

                var (isValid, claims) = _tokenValidationService
                    .ValidateToken(token);
                if (!isValid)
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    await context.Response.WriteAsync("Invalid token.");
                    return;
                }

                // If valid, proceed with WebSocket connection
                var webSocket = await context
                .WebSockets.AcceptWebSocketAsync();
                await HandleWebSocketConnection(webSocket, token);
            }
            else
            {
                // For non-WebSocket requests, pass to the next middleware
                await _next(context);
            }
        }

        private async Task
        HandleWebSocketConnection
        (System.Net.WebSockets.WebSocket webSocket, string token)
        {
            // Example WebSocket handling logic
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync
            (new ArraySegment<byte>(buffer),
            System.Threading.CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                var message = System.Text.Encoding.UTF8.
                GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received: {message}");

                var responseMessage = System.Text.Encoding
                .UTF8.GetBytes("Message received.");

                await webSocket.SendAsync(new ArraySegment<byte>
                (responseMessage), System.Net.WebSockets
                .WebSocketMessageType.Text, true,
                System.Threading.CancellationToken.None);

                result = await webSocket.ReceiveAsync
                (new ArraySegment<byte>(buffer),
                System.Threading.CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value,
            result.CloseStatusDescription, System.Threading
            .CancellationToken.None);
        }
    }
}