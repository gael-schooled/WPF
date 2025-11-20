using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;


namespace WPF_Project
{
    internal class StopWatchViewModel : INotifyPropertyChanged
    {
        private readonly DispatcherTimer _timer;
        private DateTime _startTime;
        private TimeSpan _elapsedTime;
        private bool _isRunning;

        public StopWatchViewModel()
        {
            // Initialize commands
            DemarrerCommand = new RelayCommand(Start, CanStart);
            ArreterCommand = new RelayCommand(Stop, CanStop);
            ResetCommand = new RelayCommand(Reset, CanReset);

            // Setup timer
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _timer.Tick += (s, e) => UpdateElapsedTime();

            _isRunning = false;
            UpdateButtonStates();
        }

        public TimeSpan ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                _elapsedTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SecondAngle)); // Update clock hand
            }
        }

        public double SecondAngle => ElapsedTime.TotalSeconds * 6; // 6 degrees per second

        public IRelayCommand DemarrerCommand { get; }
        public IRelayCommand ArreterCommand { get; }
        public IRelayCommand ResetCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Start()
        {
            _startTime = DateTime.Now - ElapsedTime;
            _isRunning = true;
            _timer.Start();
            UpdateButtonStates();
        }

        private void Stop()
        {
            _isRunning = false;
            _timer.Stop();
            UpdateButtonStates();
        }

        private void Reset()
        {
            _isRunning = false;
            _timer.Stop();
            ElapsedTime = TimeSpan.Zero;
            UpdateButtonStates();
        }

        private bool CanStart() => !_isRunning;
        private bool CanStop() => _isRunning;
        private bool CanReset() => !_isRunning && ElapsedTime > TimeSpan.Zero;

        private void UpdateElapsedTime()
        {
            ElapsedTime = DateTime.Now - _startTime;
        }

        private void UpdateButtonStates()
        {
            DemarrerCommand.NotifyCanExecuteChanged();
            ArreterCommand.NotifyCanExecuteChanged();
            ResetCommand.NotifyCanExecuteChanged();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
