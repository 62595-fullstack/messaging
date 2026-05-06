using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Services.MessageStream;

public static class MessageStream
{
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, Channel<object>>> _subscribers = new();

    public static (Guid connectionId, ChannelReader<object> reader) Subscribe(string userId)
    {
        Guid connId = Guid.NewGuid();
        Channel<object> channel = Channel.CreateUnbounded<object>();
        ConcurrentDictionary<Guid, Channel<object>> userChannels =
            _subscribers.GetOrAdd(userId, _ => new ConcurrentDictionary<Guid, Channel<object>>());
        userChannels[connId] = channel;
        return (connId, channel.Reader);
    }

    public static void Unsubscribe(string userId, Guid connectionId)
    {
        if (_subscribers.TryGetValue(userId, out ConcurrentDictionary<Guid, Channel<object>>? userChannels))
        {
            if (userChannels.TryRemove(connectionId, out Channel<object>? channel))
            {
                channel.Writer.TryComplete();
            }
            if (userChannels.IsEmpty)
            {
                _subscribers.TryRemove(userId, out _);
            }
        }
    }

    public static void Publish(string userId, object payload)
    {
        if (_subscribers.TryGetValue(userId, out ConcurrentDictionary<Guid, Channel<object>>? userChannels))
        {
            foreach (Channel<object> ch in userChannels.Values)
            {
                ch.Writer.TryWrite(payload);
            }
        }
    }
}
