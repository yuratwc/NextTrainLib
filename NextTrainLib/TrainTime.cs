using System;
using System.Collections.Generic;

namespace NextTrainLib
{
    public class TrainTime : IComparable<TrainTime>
    {
        private int _hour;
        private int _minute;
        private Dictionary<char, TrainAnnotation> _annotations;

        public int Hour
        {
            get => _hour;
            set => _hour = value;
        }

        public int Minute
        {
            get => _minute;
            set => _minute = value;
        }

        public Dictionary<char, TrainAnnotation> Annotations => _annotations;

        public TimeSpan TimeSpan => new TimeSpan(_hour, _minute, 0);
        public DateTime TodayTime => DateTime.Today.Add(TimeSpan);

        public TrainTime(int hour, int minute)
        {
            _hour = hour;
            _minute = minute;
            _annotations = new Dictionary<char, TrainAnnotation>();
        }

        public TrainTime()
            : this(0, 0) { }

        public void AddAnnotation(TrainAnnotation annotation)
        {
            if(_annotations.ContainsKey(annotation.Label))
            {
                _annotations[annotation.Label] = annotation;
            }
            else
            {
                _annotations.Add(annotation.Label, annotation);
            }
        }

        public void AddAnnotation(char label, string content)
        {
            AddAnnotation(new TrainAnnotation(label, content));
        }

        public int CompareTo(TrainTime other)
        {
            return (_hour * 60 + _minute) - (other._hour * 60 + other._minute);
        }
    }
}
