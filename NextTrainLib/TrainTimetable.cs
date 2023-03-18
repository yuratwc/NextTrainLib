using System.Collections.Generic;

namespace NextTrainLib
{
    public class TrainTimetable
    {
        private TrainTimetablePage[] _pages;

        private List<TrainStop> _stops;

        public TrainTimetablePage[] Pages
        {
            get => _pages;
        }

        public List<TrainStop> Stops
        {
            get => _stops;
            set => _stops = value;
        }

        public TrainTimetablePage this[TrainTimetableDay day]
        {
            get => GetPage(day);
            set => SetPage(day, value);
        }

        public TrainTimetable()
        {
            _pages = new TrainTimetablePage[9];
            _stops = new List<TrainStop>();
        }

        public TrainTimetablePage GetPage(TrainTimetableDay day)
        {
            return _pages[(int)day];
        }

        public void SetPage(TrainTimetableDay day, TrainTimetablePage page)
        {
            _pages[(int)day] = page;
        }



    }
}
