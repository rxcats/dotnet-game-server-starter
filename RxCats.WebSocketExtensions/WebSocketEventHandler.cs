using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RxCats.WebSocketExtensions.WebSocketMessageBody;

namespace RxCats.WebSocketExtensions
{
    public class WebSocketEventHandler
    {
        private readonly ILogger<WebSocketEventHandler> logger;

        private readonly WebSocketSessionFactory sessionFactory;

        public WebSocketEventHandler(ILogger<WebSocketEventHandler> logger, WebSocketSessionFactory sessionFactory)
        {
            this.logger = logger;
            this.sessionFactory = sessionFactory;
        }

        public void OnOpen(WebSocketSession session)
        {
            sessionFactory.Add(session);
        }

        public void OnClose(WebSocketSession session)
        {
            sessionFactory.Remove(session);
        }

        private WebSocketMessageRequest<T> ConvertMessage<T>(string payload)
        {
            return JsonConvert.DeserializeObject<WebSocketMessageRequest<T>>(payload);
        }

        public void Connect(WebSocketSession session, WebSocketMessageRequest<CharacterInfo> req)
        {
            logger.LogInformation("MessageType: {}", req.MessageType);
            logger.LogInformation("Message: {}", req.Message);

            session.CharacterNo = req.Message.CharacterNo;
            session.Nickname = req.Message.Nickname;
            sessionFactory.Add(session);
        }

        public void Disconnect(WebSocketSession session, string payload)
        {
            sessionFactory.Remove(session);
        }

        public void CreateGame(WebSocketSession session, string payload)
        {

        }

        public void JoinGame(WebSocketSession session, string payload)
        {
            
        }

        public void LeaveGame(WebSocketSession session, string payload)
        {
            
        }

        public void InviteGame(WebSocketSession session, string payload)
        {
            
        }

        public void ReadyGame(WebSocketSession session, string payload)
        {
            
        }

        public void StartGame(WebSocketSession session, string payload)
        {
            
        }

        public void EndGame(WebSocketSession session, string payload)
        {
            
        }

        public void GiveUpGame(WebSocketSession session, string payload)
        {
            
        }

        public async void GameChat(WebSocketSession session, WebSocketMessageRequest<GameChatMessage> req)
        {
            var res = new WebSocketMessageResponse<string>
            {
                SessionId = session.SessionId,
                MessageType = WebSocketMessageType.GameChat,
                Code = (int)WebSocketMessageResultCode.Ok,
                Result = req.Message.Message
            };

            var sessions = sessionFactory.All();

            foreach (var s in sessions)
            {
                await s.Value.SendAsyncTextMessage(res);
            }
        }
    }
}