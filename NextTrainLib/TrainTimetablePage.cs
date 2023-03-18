namespace NextTrainLib
{
    public class TrainTimetablePage
    {
        private TrainTimeList _timeList;
        private string _title;
        //private Dictionary<char, TrainAnnotation> _annotations;

        public TrainTimeList TimeList
        {
            get => _timeList;
            set => _timeList = value;
        }

        public string Title
        {
            get => _title;
            set => _title = value;
        }

        //public Dictionary<char, TrainAnnotation> Annotations
        //{
        //    get => _annotations;
        //    set => _annotations = value;
        //}

    }
}
