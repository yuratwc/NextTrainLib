using System;
using System.Collections;
using System.Collections.Generic;

namespace NextTrainLib
{
    public class TrainTimeList  : IEnumerable<TrainTime>
    {
        private List<TrainTime> _timetable;

        public TrainTime this[int index] => _timetable [index];

        public int Count => _timetable.Count;

        public TrainTimeList(int capacity = 20)
        {
            _timetable = new List<TrainTime>(capacity);
        }

        internal TrainTimeList(List<TrainTime> timetable)
        {
            _timetable = timetable;
        }

        public void Add(TrainTime time)
        {
            var index = _timetable.BinarySearch(time);

            if (index < 0)
            {
                _timetable.Insert(~index, time);
            }
            else
            {
                _timetable.Insert(index, time);
            }
        }

        public void RemoveAt(int index)
        {
            _timetable.RemoveAt(index);
        }

        public int GetNowOrAfterIndex(TimeSpan timespan)
        {
            for(var i = 0; i < _timetable.Count; i++)
            {
                if(_timetable[i].TimeSpan >= timespan)
                {
                    return i;
                }
            }

            return _timetable.Count;
        }

        public TrainTime GetNowOrAfter(TimeSpan timespan)
        {
            var index = GetNowOrAfterIndex(timespan);

            if (index < _timetable.Count)
            {
                return _timetable[index];
            }

            return null;
        }


        public int GetAfterIndex(TimeSpan timespan)
        {
            for (var i = 0; i < _timetable.Count; i++)
            {
                if (_timetable[i].TimeSpan > timespan)
                {
                    return i;
                }
            }

            return _timetable.Count;
        }

        public TrainTime GetAfter(TimeSpan timespan)
        {
            var index = GetAfterIndex(timespan);

            if(index < _timetable.Count)
            {
                return _timetable[index];
            }

            return null;
        }

        IEnumerator<TrainTime> IEnumerable<TrainTime>.GetEnumerator()
        {
            return _timetable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _timetable.GetEnumerator();
        }
    }
}
