using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RxCats.WebSocketExtensions
{
    public class CharacterInfo
    {
        public long CharacterNo { get; set; }

        public string Nickname { get; set; } = "";

        public string Avatar { get; set; } = "";
    }

    public class GameSlotInfo
    {
        public string GameName { get; set; }

        public long GameNo { get; set; }

        public long MasterCharacterNo { get; set; }

        public CharacterInfo Slot1CharacterInfo { get; set; }

        public CharacterInfo Slot2CharacterInfo { get; set; }

        public GameState GameState { get; set; } = GameState.Wait;
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GameState
    {
        Wait,
        Ready,
        Started
    }

    public class GameResultInfo
    {
        public string GameName { get; set; }

        public long GameNo { get; set; }

        public CharacterInfo Winner { get; set; }

        public CharacterInfo Loser { get; set; }

        public long WinnerScore { get; set; }

        public long LoserScore { get; set; }
    }

    public class CreateGameMessage
    {
        public string GameName { get; set; }
    }

    public class JoinGameMessage
    {
        public long GameNo { get; set; }
    }

    public class LeaveGameMessage
    {
        public long GameNo { get; set; }
    }

    public class InviteGameMessage
    {
        public long TargetCharacterNo { get; set; }

        public long GameNo { get; set; }
    }

    public class ReadyGameMessage
    {
        public long GameNo { get; set; }
    }

    public class StartGameMessage
    {
        public long GameNo { get; set; }
    }

    public class EndGameResult
    {
        public GameResultInfo GameResultInfo { get; set; }
    }

    public class GameChatMessage
    {
        public long GameNo { get; set; }

        public string Message { get; set; }
    }

    public class GameChatResult
    {
        public CharacterInfo CharacterInfo { get; set; }

        public long GameNo { get; set; }

        public string Message { get; set; }
    }
}