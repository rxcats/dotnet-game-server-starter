using System;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

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

        private MethodInfo GetMethod(JObject jsonObj)
        {
            var messageType = jsonObj["MessageType"];

            if (messageType == null)
            {
                throw new InvalidOperationException();
            }

            var type = eventHandler.GetType();

            var method = type.GetMethod(messageType.ToString());

            if (method == null)
            {
                throw new InvalidOperationException();
            }

            return method;
        }

        private object GetArgument(JObject jsonObj, Type parameterType)
        {
            return jsonObj["Message"]?.ToObject(parameterType);
        }

        public Task HandleTextMessage(WebSocketSession session, byte[] buffer)
        {
            var payload = "";
            try
            {
                payload = Encoding.UTF8.GetString(buffer);

                var jsonObj = JObject.Parse(payload);

                var method = GetMethod(jsonObj);

                var parameters = method.GetParameters();

                switch (parameters.Length)
                {
                    case 0:
                        method.Invoke(eventHandler, null);
                        break;
                    case 1:
                        method.Invoke(eventHandler, new object[] { session });
                        break;
                    default:
                    {
                        var arg = GetArgument(jsonObj, parameters[1].ParameterType);

                        logger.LogInformation("arg {}", arg);

                        try
                        {
                            method.Invoke(eventHandler, new[] { session, arg });
                        }
                        catch (Exception e)
                        {
                            logger.LogError("Invalid Operation, SessionId: {}, Payload: {}, arg: {}",
                                session.SessionId, payload, arg);
                            logger.LogError(e.StackTrace);
                            throw;
                        }

                        break;
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError("Invalid Packet, SessionId: {}, Payload: {}", session.SessionId, payload);
                logger.LogError(e.StackTrace);
                throw;
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