using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

    public class TesterViewModel : INotifyPropertyChanged
    {
        RequestSocket _socket;

        public event PropertyChangedEventHandler PropertyChanged;

        public TesterViewModel()
        {
            _socket = new RequestSocket();
            _socket.Connect("tcp://localhost:5555");
        }

        public string ResponseReceived { get; set; }

        public string FileName { get; set; } = @"c:\Users\jhenriksen\OneDrive - HBK\Data\Gear 2, WOT.bkc";

        public void CommandLoadFile()
        {
            SendCommand(_socket, "LOADFILE", JsonConvert.SerializeObject(FileName));
            var ans = AwaitResponse(_socket, "HANDLE");
            var handle = int.Parse(ans);
        }

        public void CommandSettings()
        {
            var settings = new[]
            {
                new PropertyPair { PropertyName = "NumSources", PropertyValue = "5" },
                new PropertyPair { PropertyName = "NumIndicators", PropertyValue = "7" },
            };

            SendCommand(_socket, "SETTINGS", JsonConvert.SerializeObject(settings));
            AwaitResponse(_socket, "ACK");
        }

        public void CommandFileInfo()
        {
            SendCommand(_socket, "FILEINFO", JsonConvert.SerializeObject("0"));
            AwaitResponse(_socket, "FILEINFO");
        }

        public void CommandGetData()
        {
            SendCommand(_socket, "GETDATA", JsonConvert.SerializeObject("0"));
            AwaitResponse(_socket, "DATA");
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

            ResponseReceived = str;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResponseReceived)));

            return cmd.Payload;
        }
    }
}
