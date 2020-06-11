using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using static System.Threading.Interlocked;

namespace RxCats.WebSocketExtensions
{
    public class GameSlotFactory : IDisposable
    {
        private static long _counter;

        private readonly ConcurrentDictionary<long, GameSlotInfo> slots;

        public GameSlotFactory()
        {
            slots = new ConcurrentDictionary<long, GameSlotInfo>();
        }

        private static long IncrementAndGet()
        {
            Increment(ref _counter);
            return _counter;
        }

        public GameSlotInfo GetSlot(long gameNo)
        {
            slots.TryGetValue(gameNo, out var info);
            return info;
        }

        public List<long> GetSlotCharacterNos(long gameNo)
        {
            GameSlotInfo info = GetSlot(gameNo);

            var characterNos = new List<long>();

            if (info == null)
            {
                return characterNos;
            }

            if (info.Slot1CharacterInfo != null)
            {
                characterNos.Add(info.Slot1CharacterInfo.CharacterNo);
            }

            if (info.Slot2CharacterInfo != null)
            {
                characterNos.Add(info.Slot2CharacterInfo.CharacterNo);
            }

            return characterNos;
        }

        public GameSlotInfo AddSlot(CharacterInfo characterInfo, string gameName = "")
        {
            var gameNo = IncrementAndGet();

            var info = new GameSlotInfo
            {
                GameNo = gameNo,
                GameName = gameName,
                MasterCharacterNo = characterInfo.CharacterNo,
                Slot1CharacterInfo = characterInfo
            };

            slots.TryAdd(gameNo, info);

            return info;
        }

        public GameSlotInfo FindAndException(long gameNo)
        {
            if (slots.TryGetValue(gameNo, out var info))
            {
                return info;
            }

            throw new ServiceException("Cannot Find GameNo");
        }

        public GameSlotInfo SearchSlot(CharacterInfo characterInfo, string gameName = "")
        {
            foreach (var slot in slots)
            {
                if (!IsFull(slot.Value))
                {
                    return AddSlotMember(slot.Value.GameNo, characterInfo);
                }
            }

            return AddSlot(characterInfo, gameName);
        }

        public bool IsFull(GameSlotInfo info)
        {
            return info.Slot1CharacterInfo != null && info.Slot2CharacterInfo != null;
        }

        public GameSlotInfo AddSlotMember(long gameNo, CharacterInfo characterInfo)
        {
            GameSlotInfo info = FindAndException(gameNo);

            if (IsFull(info))
            {
                throw new ServiceException("GameSlot Is Full");
            }

            var modify = new GameSlotInfo
            {
                GameNo = info.GameNo,
                GameName = info.GameName,
                MasterCharacterNo = info.MasterCharacterNo,
            };

            if (info.Slot1CharacterInfo == null)
            {
                modify.Slot1CharacterInfo = characterInfo;
                modify.Slot2CharacterInfo = info.Slot2CharacterInfo;
            }
            else
            {
                modify.Slot1CharacterInfo = info.Slot1CharacterInfo;
                modify.Slot2CharacterInfo = characterInfo;
            }

            slots.TryUpdate(gameNo, modify, info);

            return modify;
        }

        public GameSlotInfo RemoveSlotMember(long gameNo, CharacterInfo characterInfo)
        {
            if (!slots.TryGetValue(gameNo, out GameSlotInfo info)) throw new ServiceException("Cannot Find GameNo");
            
            // 참가자가 1명인 경우 방 제거
            if (info.Slot1CharacterInfo == null || info.Slot2CharacterInfo == null)
            {
                RemoveSlot(gameNo);
                return null;
            }

            CharacterInfo slot1CharacterInfo = null;
            CharacterInfo slot2CharacterInfo = null;
            long masterCharacterNo = 0;

            if (info.Slot1CharacterInfo.CharacterNo == characterInfo.CharacterNo)
            {
                slot2CharacterInfo = info.Slot2CharacterInfo;
                masterCharacterNo = info.Slot2CharacterInfo.CharacterNo;
            }
            else if (info.Slot2CharacterInfo.CharacterNo == characterInfo.CharacterNo)
            {
                slot1CharacterInfo = info.Slot1CharacterInfo;
                masterCharacterNo = info.Slot1CharacterInfo.CharacterNo;
            }

            var modify = new GameSlotInfo
            {
                GameNo = info.GameNo,
                GameName = info.GameName,
                MasterCharacterNo = masterCharacterNo,
                Slot1CharacterInfo = slot1CharacterInfo,
                Slot2CharacterInfo = slot2CharacterInfo
            };

            slots.TryUpdate(gameNo, modify, info);

            return modify;
        }

        public void RemoveSlot(long gameNo)
        {
            slots.TryRemove(gameNo, out _);
        }

        public void Dispose()
        {
            slots.Clear();
        }
    }
}