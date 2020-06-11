using System;
using System.Collections.Concurrent;

namespace RxCats.WebSocketExtensions
{
    public class WebSocketSessionFactory : IDisposable
    {
        private readonly ConcurrentDictionary<long, WebSocketSession> sessions;

        public WebSocketSessionFactory()
        {
            sessions = new ConcurrentDictionary<long, WebSocketSession>();
        }

        public void Add(WebSocketSession session)
        {
            if (session.CharacterNo == 0)
            {
                return;
            }

            sessions.TryAdd(session.CharacterNo, session);
        }

        public void Remove(WebSocketSession session)
        {
            sessions.TryRemove(session.CharacterNo, out _);
        }

        public ConcurrentDictionary<long, WebSocketSession> All()
        {
            return sessions;
        }

        public WebSocketSession GetByCharacterNo(long characterNo)
        {
            sessions.TryGetValue(characterNo, out var session);
            return session;
        }

        public void Dispose()
        {
            sessions.Clear();
        }
    }
}