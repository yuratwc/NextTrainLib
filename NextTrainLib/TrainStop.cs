using System.Collections.Generic;

namespace NextTrainLib
{
    public class TrainStop
    {
        private const char LocalTrainLabel = '=';
        private const int DurationNonStop = -1;

        private string _name;
        private string _transferTblFile;
        private string _transferCfgFile;
        private int _transferTime;
        private Dictionary<char, int> _durations;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public string TransferTblFile
        {
            get => _transferTblFile;
            set => _transferTblFile = value;
        }

        public string TransferCfgFile
        {
            get => _transferCfgFile;
            set => _transferCfgFile = value;
        }

        public int TransferTime
        {
            get => _transferTime;
            set => _transferTime = value;
        }

        public TrainStop()
        {
            _durations = new Dictionary<char, int>();
        }

        public bool ContainsDurationLabel(char label)
        {
            return _durations.ContainsKey(label);
        }
        
        public int GetDuration(char label)
        {
            if(_durations.ContainsKey(label))
            {
                return _durations[label];
            }
            return 0;
        }
        public int GetDuration()
        {
            return GetDuration(LocalTrainLabel);
        }

        public bool IsTrainStop(char label)
        {
            return GetDuration(label) >= 0;
        }

        public bool IsTrainStop()
        {
            return IsTrainStop(LocalTrainLabel);
        }

        public void AddDuration(char label, int minute)
        {
            if (_durations.ContainsKey(label))
            {
                _durations[label] = minute;
            }
            else
            {
                _durations.Add(label, minute);
            }
        }

        public void AddDuration(int minute)
        {
            AddDuration(LocalTrainLabel, minute);
        }

        public void RemoveDuration(char label)
        {
            if(_durations.ContainsKey(label))
            {
                _durations.Remove(label);
            }
        }

        public void RemoveDuration()
        {
            RemoveDuration(LocalTrainLabel);
        }
    }
}
