using System.Collections.Generic;

namespace RxCats.WebSocketExtensions.WebSocketMessageBody
{
    public class CharacterInfo
    {
        public long CharacterNo { get; set; } = 0;

        public string Nickname { get; set; } = "";
    }

    public class GameInfo
    {
        public string GameName { get; set; }

        public long GameNo { get; set; }

        public CharacterInfo Slot1CharacterInfo { get; set; }

        public CharacterInfo Slot2CharacterInfo { get; set; }

        public GameState GameState { get; set; } = GameState.Wait;
    }

    public enum GameState
    {
        Wait, Ready, Started
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

    public class ConnectMessage
    {
        public CharacterInfo Character { get; set; }
    }

    public class DisconnectMessage
    {
        public long CharacterNo { get; set; }
    }

    public class CreateGameMessage
    {
        public string GameName { get; set; }
    }

    public class CreateGameResult
    {
        public GameInfo GameInfo { get; set; }
    }

    public class JoinGameMessage
    {
        public long GameNo { get; set; }
    }

    public class JoinGameResult
    {
        public GameInfo GameInfo { get; set; }
    }

    public class LeaveGameMessage
    {
        public long GameNo { get; set; }
    }

    public class LeaveGameResult
    {
        public GameInfo GameInfo { get; set; }
    }

    public class InviteGameMessage
    {
        public long CharacterNo { get; set; }

        public long TargetCharacterNo { get; set; }

        public long GameNo { get; set; }
    }

    public class ReadyGameMessage
    {
        public long CharacterNo { get; set; }

        public long GameNo { get; set; }
    }

    public class ReadyGameResult
    {
        public GameInfo GameInfo { get; set; }
    }

    public class StartGameMessage
    {
        public long CharacterNo { get; set; }

        public long GameNo { get; set; }
    }

    public class StartGameResult
    {
        public GameInfo GameInfo { get; set; }
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
        public long CharacterNo { get; set; }

        public long GameNo { get; set; }

        public string Message { get; set; }
    }
}