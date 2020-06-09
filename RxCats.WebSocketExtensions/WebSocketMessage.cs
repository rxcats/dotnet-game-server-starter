using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RxCats.WebSocketExtensions
{
    public enum WebSocketMessageResultCode : int
    {
        Ok = 0,

        Error = 999999
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum WebSocketMessageType
    {
        None, Ping, Pong, Connect, Disconnect, CreateGame, JoinGame, LeaveGame, InviteGame, ReadyGame, StartGame, EndGame, GiveUpGame, GameChat
    }
    
    public class WebSocketMessageRequest<T>
    {
        public WebSocketMessageType MessageType { get; set; }

        public T Message { get; set; }
    }

    public class WebSocketMessageResponse<T>
    {
        public string SessionId { get; set; }

        public WebSocketMessageType MessageType { get; set; }

        public int Code { get; set; }

        public T Result { get; set; }

        public long Timestamp
        {
            get
            {
                return DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }
        }
    }
}