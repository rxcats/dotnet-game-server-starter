using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RxCats.WebSocketExtensions
{
    public enum WebSocketMessageResultCode
    {
        Ok = 0,

        Error = 999999
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum WebSocketMessageType
    {
        None,
        Ping,
        Pong,
        ErrorResult,

        Connect,
        Disconnect,
        CreateGame,
        JoinGame,
        SearchAndJoinGame,
        LeaveGame,
        InviteGame,
        ReadyGame,
        StartGame,
        EndGame,
        GiveUpGame,
        GameChat,
        
        ConnectResult,
        DisconnectResult,
        CreateGameResult,
        LeaveGameResult,
        GameChatResult,
        JoinGameResult
    }

    public class WebSocketMessageRequest<T>
    {
        public WebSocketMessageType MessageType { get; set; }

        public T Message { get; set; }
    }

    public class WebSocketMessageResponse<T>
    {
        public WebSocketMessageType ResultType { get; set; }

        public int Code { get; set; } = 0;

        public T Result { get; set; }

        public long Timestamp => DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
}