using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace src
{
    public class FileQueue
    {
        public static FileQueue Spawn(CancellationToken token)
        {
            var fileQueue = new FileQueue();
            Task.Run(async () => await fileQueue.WorkLoopAsync(token));
            return fileQueue;
        }
        private readonly Queue<string> _q;
        private readonly object syncroot = new object();
        public FileQueue()
        {
            _q = new Queue<string>();
        }

        private async Task WorkLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_q.Count > 0)
                {
                    var items = Enumerable.Empty<string>();
                    lock (syncroot)
                    {
                        items = _q.ToList();
                        _q.Clear();
                    }
                    if (items.Any())
                    {
                        File.AppendAllLines("/logs/web.log", items);
                    }
                }
                await Task.Delay(100);
            }
        }

        private string FlattenObject(object obj)
        {
            JObject jsonObject = JObject.FromObject(obj);
            IEnumerable<JToken> jTokens = jsonObject.Descendants().Where(p => p.Count() == 0);
            Dictionary<string, string> results = jTokens.Aggregate(new Dictionary<string, string>(),
                (properties, jToken) =>
                {
                    properties.Add(jToken.Path, jToken.ToString());
                    return properties;
                });
            var sb = new StringBuilder();

            foreach (var kvp in results)
            {
                sb.Append(kvp.Key).Append("=").Append(kvp.Value).Append("\t");
            }
            return sb.ToString();
        }
        public void Add(string str)
        {
            lock (syncroot)
            {
                _q.Enqueue(str);
            }
        }

        public void Add(object obj)
        {
            lock (syncroot)
            {
                _q.Enqueue(FlattenObject(obj));
            }
        }
    }
}