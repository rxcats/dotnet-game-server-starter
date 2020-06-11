using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RxCats.WebSocketExtensions
{
    public class WebSocketSession
    {
        public string SessionId { get; }

        public long CharacterNo { get; set; }

        public string Nickname { get; set; } = "";
        
        public string Avatar { get; set; } = "";
        
        public long JoinGameNo { get; set; }

        private readonly ConcurrentDictionary<string, object> attributes;

        private readonly WebSocket webSocket;

        public WebSocketSession(WebSocket webSocket, string sessionId)
        {
            this.webSocket = webSocket;
            SessionId = sessionId;
            attributes = new ConcurrentDictionary<string, object>();
        }

        public ConcurrentDictionary<string, object> GetAttributes()
        {
            return attributes;
        }

        public bool IsOpen()
        {
            return webSocket.State == WebSocketState.Open;
        }

        public bool IsJoinedGame()
        {
            return JoinGameNo > 0;
        }

        public void LeaveGame()
        {
            JoinGameNo = 0;
        }

        public Task SendAsyncTextMessage<T>(WebSocketMessageResponse<T> message)
        {
            if (!IsOpen())
            {
                return Task.CompletedTask;
            }

            var json = JsonConvert.SerializeObject(message);
            var bytes = Encoding.UTF8.GetBytes(json);
            return webSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length),
                System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public Task SendAsyncErrorMessage(string message = "", WebSocketMessageResultCode resultCode = WebSocketMessageResultCode.Error)
        {
            var res = new WebSocketMessageResponse<string>
            {
                ResultType = WebSocketMessageType.ErrorResult,
                Code = (int)resultCode,
                Result = message
            };
            return SendAsyncTextMessage(res);
        }

        public Task SendAsyncPong()
        {
            var res = new WebSocketMessageResponse<string>
            {
                ResultType = WebSocketMessageType.Pong,
                Result = WebSocketMessageType.Pong.ToString()
            };
            return SendAsyncTextMessage(res);
        }

        public Task SendAsyncPing()
        {
            var res = new WebSocketMessageResponse<string>
            {
                ResultType = WebSocketMessageType.Ping,
                Result = WebSocketMessageType.Ping.ToString()
            };
            return SendAsyncTextMessage(res);
        }
    }
}