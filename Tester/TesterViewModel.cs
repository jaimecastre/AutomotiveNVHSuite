using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using AutomotiveNVHSuite;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;

namespace Tester
{

    public class TesterViewModel
    {
        RequestSocket _socket;
        public TesterViewModel()
        {
            _socket = new RequestSocket();
            _socket.Connect("tcp://localhost:5555");
        }

        public void CommandLoadFile(string name)
        {
            SendCommand(_socket, "LOADFILE", JsonConvert.SerializeObject(name));
            var ans = AwaitResponse(_socket, "HANDLE");
            var handle = int.Parse(ans);
        }

        public void CommandSettings()
        {
            var settings = new[]
            {
                new PropertyPair { PropertyName = "NumSources", PropertyValue = "5" },
                new PropertyPair { PropertyName = "NumIndicators", PropertyValue = "7" }
            };

            SendCommand(_socket, "SETTINGS", JsonConvert.SerializeObject(settings));
            AwaitResponse(_socket, "ACK");
        }

        private void SendCommand(RequestSocket socket, string command, string payload)
        {
            var cmd = new CommandAndPayload
            {
                Command = command,
                Payload = payload
            };
            socket.SendFrame(JsonConvert.SerializeObject(cmd));
        }

        private string AwaitResponse(RequestSocket socket, string response)
        {
            var str = socket.ReceiveFrameString();
            var cmd = JsonConvert.DeserializeObject<CommandAndPayload>(str);

            if (cmd.Command != response)
                throw new InvalidOperationException();

            return cmd.Payload;
        }
    }
}
