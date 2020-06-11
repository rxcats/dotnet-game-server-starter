using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace RxCats.WebSocketExtensions
{
    public class WebSocketMessageHandler
    {
        private readonly RequestDelegate next;

        private readonly ILogger<WebSocketMessageHandler> logger;

        private readonly TextWebSocketHandler textWebSocketHandler;

        private const int BufferSize = 4096;

        public WebSocketMessageHandler(RequestDelegate next, ILogger<WebSocketMessageHandler> logger,
            TextWebSocketHandler textWebSocketHandler)
        {
            this.next = next;
            this.logger = logger;
            this.textWebSocketHandler = textWebSocketHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    try
                    {
                        await Listen(context);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e.StackTrace);
                    }
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                await next(context);
            }
        }

        private async Task Listen(HttpContext context)
        {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

            var webSocketSession = new WebSocketSession(webSocket, context.Connection.Id);

            await textWebSocketHandler.AfterConnectionEstablished(webSocketSession);

            var buffer = new byte[BufferSize];

            WebSocketReceiveResult result =
                await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                try
                {
                    await textWebSocketHandler.HandleTextMessage(webSocketSession, buffer);
                }
                catch (ServiceException e)
                {
                    await webSocketSession.SendAsyncErrorMessage(e.Message, e.ResultCode);
                    logger.LogError(e.StackTrace);
                }
                catch (Exception e)
                {
                    await webSocketSession.SendAsyncErrorMessage(e.Message);
                    logger.LogError(e.StackTrace);
                }
                finally
                {
                    buffer = new byte[BufferSize];
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

            await textWebSocketHandler.AfterConnectionClosed(webSocketSession, result.CloseStatus.Value);
        }
    }
}