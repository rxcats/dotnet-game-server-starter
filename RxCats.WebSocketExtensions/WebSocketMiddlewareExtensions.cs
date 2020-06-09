using Microsoft.AspNetCore.Builder;

namespace RxCats.WebSocketExtensions
{
    public static class WebSocketMiddlewareExtensions
    {
        public static IApplicationBuilder UseWebSocketMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<WebSocketMessageHandler>();
        }
    }
}
