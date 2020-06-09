using System.Collections.Concurrent;

namespace RxCats.WebSocketExtensions
{
    public class GameSlotInfo
    {
        public long Slot1 { get; set; } = 0;

        public long Slot2 { get; set; } = 0;
    }

    public class GameSlotFactory
    {
        private static readonly ConcurrentDictionary<long, GameSlotInfo> slots = new ConcurrentDictionary<long, GameSlotInfo>();

        public static void AddSlot(long gameNo)
        {
            
        }

        public static void RemoveSlot(long gameNo)
        {

        }

    }
}