using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AutomotiveNVHSuite
{
    public class CalculationsViewModel : INotifyPropertyChanged
    {
        bool _run = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public void StartRemote()
        {
            StartListenerAsync();
        }

        public void StopRemote()
        {
            _run = false;
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                Notify(nameof(Status));
            }
        }
        private string _status;

        private void Notify(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private async void StartListenerAsync()
        {
            await Task.Run(Listener);
        }

        private void Listener()
        {
            using (var responder = new ResponseSocket())
            {
                responder.Bind("tcp://*:5555");

                _run = true;
                while (_run)
                {
                    string str = responder.ReceiveFrameString();

                    var cmd = JsonConvert.DeserializeObject<CommandAndPayload>(str);
                    
                    switch (cmd.Command)
                    {
                        case "LOADFILE":
                            LoadFile(cmd.Payload);
                            break;

                        case "SETTINGS":
                            Settings(cmd.Payload);
                            break;

                        default:
                            break;
                    }

                    Status = "Running";
                    Thread.Sleep(1000);
                    Status = "Finished";
                    responder.SendFrame("ACK");
                }
            }
        }

        private void LoadFile(string payload)
        {
            var filename = JsonConvert.DeserializeObject<string>(payload);
        }

        private void Settings(string payload)
        {
            var settings = JsonConvert.DeserializeObject<PropertyPair[]>(payload);
        }
    }
}
