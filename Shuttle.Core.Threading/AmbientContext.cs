using System.Collections.Concurrent;
using System.Threading;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Threading
{
    // shout-out to Daniel Cazzulino: http://www.cazzulino.com/callcontext-netstandard-netcore.html
    public static class AmbientContext
    {
        private static readonly ConcurrentDictionary<string, AsyncLocal<object>> State = new ConcurrentDictionary<string, AsyncLocal<object>>();

        public static void SetData(string name, object data)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            State.GetOrAdd(name, _ => new AsyncLocal<object>()).Value = data;
        }

        public static object GetData(string name)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));

            return State.TryGetValue(name, out var data) ? data.Value : null;
        }
    }
}