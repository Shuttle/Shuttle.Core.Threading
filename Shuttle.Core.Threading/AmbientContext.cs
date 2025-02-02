using System.Collections.Concurrent;
using System.Threading;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading;

// shout-out to Daniel Cazzulino: http://www.cazzulino.com/callcontext-netstandard-netcore.html
public static class AmbientContext
{
    private static readonly ConcurrentDictionary<string, AsyncLocal<object>> State = new();

    public static object? GetData(string name)
    {
        return State.TryGetValue(Guard.AgainstNullOrEmptyString(name), out var data) ? data.Value : null;
    }

    public static void SetData(string name, object data)
    {
        State.GetOrAdd(Guard.AgainstNullOrEmptyString(name), _ => new()).Value = data;
    }
}