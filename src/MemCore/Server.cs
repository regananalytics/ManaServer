using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace MemCore
{
    public class MemQServer
    {
        private readonly bool _verbose;
        private readonly string _address;
        private readonly Func<string> _getMessage;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly PublisherSocket _socket;

        public int Interval = 1000;

        public MemQServer(string address, Func<string> getMessage, bool verbose = true)
        {
            _verbose = verbose;
            _address = address;
            _getMessage = getMessage;
            _cancellationTokenSource = new CancellationTokenSource();
            _socket = new PublisherSocket();
        }

        public void Start()
        {
            _socket.Bind(_address);
            Task.Run(() => PublishMessages(_cancellationTokenSource.Token));
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _socket.Close();
        }

        public async Task PublishMessages(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = _getMessage();

                _socket.SendFrame(message);
                if (_verbose)
                    Console.WriteLine(message.ToString());

                await Task.Delay(Interval, cancellationToken);
            }

        }
    }
}