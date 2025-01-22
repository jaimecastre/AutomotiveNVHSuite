using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using BK.Platform.Data.DataAccess;
using BK.Platform.Data.DataAccess.Internal;
using BK.Platform.Data.DataModel;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AutomotiveNVHSuite
{
    public class CalculationsViewModel : INotifyPropertyChanged, IDisposable
    {
        bool _run = false;
        IDataModel _dm;
        IDataAccessFactory _daf;

        int _handleNext = 0;
        IDictionary<int, IEnumerable<IActionDataProducing>> _actions = new Dictionary<int, IEnumerable<IActionDataProducing>>();

        public event PropertyChangedEventHandler PropertyChanged;

        public CalculationsViewModel()
        {
            CreateDataModel();
            StartRemote();
        }

        public void Dispose()
        {
            _dm.Dispose();
        }

        public System.Collections.ObjectModel.ObservableCollection<PropertyPair> Properties { get; } = new System.Collections.ObjectModel.ObservableCollection<PropertyPair>();

        public string CommandReceived { get; set; }

        public Dispatcher Dispatcher { get; set; }

        private void StartRemote()
        {
            StartListenerAsync();
        }

        private void CreateDataModel()
        {
            _dm = DataModelFactory.CreateMemoryDataModel("DataModel");
            _daf = _dm.GetDataAccessFactory();
        }

        //public void StopRemote()
        //{
        //    _run = false;
        //}

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
            using (var socket = new ResponseSocket())
            {
                socket.Bind("tcp://*:5555");

                _run = true;
                while (_run)
                {
                    string str = socket.ReceiveFrameString();
                    CommandReceived = str;
                    Notify(nameof(CommandReceived));

                    var cmd = JsonConvert.DeserializeObject<CommandAndPayload>(str);
                    bool responded = false;
                    switch (cmd.Command)
                    {
                        case "LOADFILE":
                            responded = LoadFile(socket, cmd.Payload);
                            break;

                        case "FILEINFO":
                            responded = FileInfo(socket, cmd.Payload);
                            break;

                        case "SETTINGS":
                            Settings(socket, cmd.Payload);
                            break;

                        default:
                            break;
                    }

                    Status = "Running";
                    Status = "Finished";

                    if (!responded)
                        SendResponse(socket, "ACK");
                }
            }
        }

        private void SendResponse(ResponseSocket socket, string msg, string payload = "")
        {
            var pkg = new CommandAndPayload
            {
                Command = msg,
                Payload = payload,
            };
            socket.SendFrame(JsonConvert.SerializeObject(pkg));
        }

        private bool LoadFile(ResponseSocket socket, string payload)
        {
            var filename = JsonConvert.DeserializeObject<string>(payload);
            int handle = _handleNext++;

            var actions = _daf.Import(filename, null, DataFormatBase.BKCommonFormatName, ImportMode.Link, null);
            _actions[handle] = actions;

            SendResponse(socket, "HANDLE", handle.ToString());
            return true;
        }

        private bool FileInfo(ResponseSocket socket, string payload)
        {
            var handle = JsonConvert.DeserializeObject<int>(payload);

            var actions = _actions[handle];
            var seqs = actions.SelectMany(x => x.OutputGroups.GetDataSequences(true));
            var response = new FileInformation()
            {
                NumSequences = seqs.Count(),
                SequenceNames = seqs.Select(x => x.Name).ToArray()
            };

            SendResponse(socket, "FILEINFO", JsonConvert.SerializeObject(response));

            return true;
        }

        private bool Settings(ResponseSocket socket, string payload)
        {
            var settings = JsonConvert.DeserializeObject<PropertyPair[]>(payload);

            foreach (var setting in settings)
            {
                Dispatcher.Invoke(() =>
                {

                    if (Properties.Any(x => x.PropertyName.Equals(setting.PropertyName)))
                    {
                        var found = Properties.Single(x => x.PropertyName.Equals(setting.PropertyName));
                        Properties.Remove(found);
                    }
                    Properties.Add(setting);
                    Notify(nameof(Properties));
                });
            }

            return false;
        }
    }
}
