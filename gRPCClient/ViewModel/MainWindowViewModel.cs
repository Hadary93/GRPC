using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Wpf.Ui.Common;

namespace gRPCClient.ViewModel
{
    public class MainWindowViewModel :  INotifyPropertyChanged
    {
        #region Inotify essentials
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region properties
        private string serverResponse = string.Empty;
        private string input = string.Empty;
        private string errors = string.Empty;
        private SolidColorBrush color  = new();
        public string ServerResponse
        {
            get { return serverResponse; }
            set
            {
                serverResponse = value;
                OnPropertyChanged();
            }
        }
        public string Input
        {
            get { return input; }
            set
            {
                input = value;
                OnPropertyChanged();
            }
        }
        public string Errors
        {
            get { return errors; }
            set
            {
                errors = value;
                OnPropertyChanged();
            }
        }
        public SolidColorBrush Color
        {
            get { return color; }
            set
            {
                color = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<KeyValuePair<string,string>> output
        {
            get;
            set;
        }
        #endregion

        #region commands
        public ICommand convert { get; set; }
        #endregion

        #region constructor
        public MainWindowViewModel()
        {
            convert = new RelayCommand(ExcuteConvert);
            output = new();
            Color = System.Windows.Media.Brushes.LightGray;
            CheckServer();
        }
        #endregion

        #region functions
        private async void ExcuteConvert(object parameter)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5212");
            var client = new Greeter.GreeterClient(channel);
            try
            {
                var reply = await client.SayHelloAsync(new HelloRequest { Name = input });
                output.Add(new(Input,reply.Message));
                Errors = string.Empty;
                Color = System.Windows.Media.Brushes.LightGreen;
            }
            catch (Exception error)
            {
                Errors= error.Message;
                Color = System.Windows.Media.Brushes.Red;
            }
        }
        /// <summary>
        /// check the avalaibility of the server
        /// </summary>
        private async void CheckServer()
        {
            while (true)
            {
                // Call a method to check the server status
                Color = await CheckServerStatusAsync();
                await Task.Delay(50);
            }
        }
        private async Task<SolidColorBrush> CheckServerStatusAsync()
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5212");
            var client = new Greeter.GreeterClient(channel);
            SolidColorBrush color;
            try
            {
                var reply = await client.SayHelloAsync(new HelloRequest { Name = "10" });
                color = System.Windows.Media.Brushes.LightGreen;
            }
            catch (Exception error)
            {
                color = System.Windows.Media.Brushes.Red;
            }
            return color;
        }
        #endregion
    }
}
