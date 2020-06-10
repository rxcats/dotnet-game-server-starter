using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
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

                JObject jsonObj = JObject.Parse(payload);

                string methodName = jsonObj["MessageType"].ToString();

                Type type = eventHandler.GetType();

                MethodInfo method = type.GetMethod(methodName);

                ParameterInfo[] paramters = method.GetParameters();

                if (paramters.Length == 0)
                {
                    method.Invoke(eventHandler, null);
                }
                else if (paramters.Length == 1)
                {
                    method.Invoke(eventHandler, new object[] { session });
                }
                else
                {
                    Type pType = paramters[1].ParameterType;
                    
                    object arg = jsonObj["Message"].ToObject(pType);

                    logger.LogInformation("arg {}", arg);

                    try
                    {
                        method.Invoke(eventHandler, new object[] { session, arg });
                    }
                    catch (Exception e)
                    {
                        logger.LogError("Invalid Operation, SessionId: {}, Payload: {}, ParameterType: {}, arg: {}", session.SessionId, payload, pType, arg);
                        logger.LogError(e.StackTrace);
                        throw e;
                    }
                    
                }
            }
            catch (Exception e)
            {
                logger.LogError("Invalid Packet, SessionId: {}, Payload: {}", session.SessionId, payload);
                logger.LogError(e.StackTrace);
                throw e;
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