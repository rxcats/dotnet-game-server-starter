using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RxCats.WebSocketExtensions
{
    public class WebSocketEventHandler
    {
        private readonly ILogger<WebSocketEventHandler> logger;

        private readonly WebSocketSessionFactory sessionFactory;

        private readonly GameSlotFactory gameSlotFactory;

        public WebSocketEventHandler(ILogger<WebSocketEventHandler> logger, WebSocketSessionFactory sessionFactory,
            GameSlotFactory gameSlotFactory)
        {
            this.logger = logger;
            this.sessionFactory = sessionFactory;
            this.gameSlotFactory = gameSlotFactory;
        }

        private async void SendMessageByCharacterNos<T>(List<long> characterNos, WebSocketMessageResponse<T> res)
        {
            foreach (var characterNo in characterNos)
            {
                var session = sessionFactory.GetByCharacterNo(characterNo);
                if (session != null)
                {
                    await session.SendAsyncTextMessage(res);
                }
            }
        }

        private async void BroadCastMessage<T>(long gameNo, WebSocketMessageResponse<T> res)
        {
            var characterNos = gameSlotFactory.GetSlotCharacterNos(gameNo);

            foreach (var characterNo in characterNos)
            {
                var session = sessionFactory.GetByCharacterNo(characterNo);
                if (session != null)
                {
                    await session.SendAsyncTextMessage(res);
                }
            }
        }

        private async void SendMessage<T>(WebSocketSession session, WebSocketMessageResponse<T> res)
        {
            await session.SendAsyncTextMessage(res);
        }

        public CharacterInfo GetCharacterInfoFromSession(WebSocketSession session)
        {
            return new CharacterInfo
            {
                CharacterNo = session.CharacterNo,
                Nickname = session.Nickname
            };
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

        public void Connect(WebSocketSession session, CharacterInfo req)
        {
            session.CharacterNo = req.CharacterNo;
            session.Nickname = req.Nickname;
            sessionFactory.Add(session);

            var res = new WebSocketMessageResponse<string>
            {
                ResultType = WebSocketMessageType.ConnectResult,
            };

            SendMessage(session, res);
        }

        public void Disconnect(WebSocketSession session, CharacterInfo req)
        {
            sessionFactory.Remove(session);

            var res = new WebSocketMessageResponse<string>
            {
                ResultType = WebSocketMessageType.DisconnectResult,
            };

            SendMessage(session, res);
        }

        public void CreateGame(WebSocketSession session, CreateGameMessage req)
        {
            CharacterInfo characterInfo = GetCharacterInfoFromSession(session);

            GameSlotInfo result = gameSlotFactory.AddSlot(characterInfo, req.GameName);

            var res = new WebSocketMessageResponse<GameSlotInfo>
            {
                ResultType = WebSocketMessageType.CreateGameResult,
                Result = result
            };

            SendMessage(session, res);
        }

        public void JoinGame(WebSocketSession session, JoinGameMessage req)
        {
            CharacterInfo characterInfo = GetCharacterInfoFromSession(session);

            GameSlotInfo result = gameSlotFactory.AddSlotMember(req.GameNo, characterInfo);

            var res = new WebSocketMessageResponse<GameSlotInfo>
            {
                ResultType = WebSocketMessageType.JoinGameResult,
                Result = result
            };

            BroadCastMessage(req.GameNo, res);
        }

        public void SearchAndJoinGame(WebSocketSession session)
        {
            CharacterInfo characterInfo = GetCharacterInfoFromSession(session);

            GameSlotInfo result = gameSlotFactory.SearchSlot(characterInfo);

            BroadCastMessage(result.GameNo, new WebSocketMessageResponse<GameSlotInfo>
            {
                ResultType = WebSocketMessageType.JoinGameResult,
                Result = result
            });
        }

        public void LeaveGame(WebSocketSession session, LeaveGameMessage req)
        {
            CharacterInfo characterInfo = GetCharacterInfoFromSession(session);

            GameSlotInfo result = gameSlotFactory.RemoveSlotMember(req.GameNo, characterInfo);

            if (result != null)
            {
                var masterSession = sessionFactory.GetByCharacterNo(result.MasterCharacterNo);

                var res = new WebSocketMessageResponse<GameSlotInfo>
                {
                    ResultType = WebSocketMessageType.LeaveGameResult,
                    Result = result
                };

                SendMessage(masterSession, res);
            }

            SendMessage(session, new WebSocketMessageResponse<GameSlotInfo>
            {
                ResultType = WebSocketMessageType.LeaveGameResult
            });
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

        public void GameChat(WebSocketSession session, GameChatMessage req)
        {
            var result = new GameChatResult
            {
                CharacterInfo = GetCharacterInfoFromSession(session),
                GameNo = req.GameNo,
                Message = req.Message
            };

            var res = new WebSocketMessageResponse<GameChatResult>
            {
                ResultType = WebSocketMessageType.GameChatResult,
                Result = result
            };

            BroadCastMessage(req.GameNo, res);
        }
    }
}