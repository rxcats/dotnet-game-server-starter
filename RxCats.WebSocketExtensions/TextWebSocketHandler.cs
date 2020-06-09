using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;
using System;
using System.Text;

namespace RxCats.WebSocketExtensions
{
    public class TextWebSocketHandler
    {
        private readonly ILogger<TextWebSocketHandler> logger;

        private readonly WebSocketEventHandler eventHandler;

        public TextWebSocketHandler(ILogger<TextWebSocketHandler> logger, WebSocketEventHandler eventHandler)
        {
            this.logger = logger;
            this.eventHandler = eventHandler;
        }

        public Task AfterConnectionEstablished(WebSocketSession session)
        {
            logger.LogInformation("SessionId: {}, ConnectionEstablished.", session.SessionId);
            eventHandler.OnOpen(session);
            return Task.CompletedTask;
        }

        public Task HandleTextMessage(WebSocketSession session, byte[] buffer)
        {
            string payload = "";
            try
            {
                payload = Encoding.UTF8.GetString(buffer);

                var req = JsonConvert.DeserializeObject<WebSocketMessageRequest<object>>(payload);

                logger.LogInformation("req {}", req);

                Type type = typeof(WebSocketEventHandler);

                logger.LogInformation("type {}", type);

                MethodInfo method = type.GetMethod(req.MessageType.ToString());

                logger.LogInformation("method {}", method);

                ParameterInfo[] paramters = method.GetParameters();

                if (paramters.Length == 0)
                {
                    method.Invoke(eventHandler, new object[] { });
                }
                else if (paramters.Length == 1)
                {
                    method.Invoke(eventHandler, new object[] { session });
                }
                else
                {
                    Type pType = paramters[1].ParameterType;
                    var arg = JsonConvert.DeserializeObject(payload, pType);
                    method.Invoke(eventHandler, new object[] { session, arg });
                }
            }
            catch (Exception e)
            {
                logger.LogError("Invalid Packet, SessionId: {}, Payload: {}", session.SessionId, payload);
                logger.LogError(e.Message);
            }

            return Task.CompletedTask;
        }

        public Task AfterConnectionClosed(WebSocketSession session, WebSocketCloseStatus closedStatus)
        {
            logger.LogInformation("SessionId: {}, Connection Closed ({}).", session.SessionId, closedStatus);
            eventHandler.OnClose(session);
            return Task.CompletedTask;
        }
    }
}