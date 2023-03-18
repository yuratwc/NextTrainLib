using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NextTrainLib
{
    public class TblReader
    {
        private TextReader _reader;

        private Dictionary<char, TrainAnnotation> _annotations;
        private List<TrainTimetablePage> _timetables;
        private int[] _timetableMap;
        private List<TrainStop> _stops;

        private TblReader(TextReader reader)
        {
            _reader = reader;
            _annotations = new Dictionary<char, TrainAnnotation>();
            _timetables = new List<TrainTimetablePage>();
            _stops = new List<TrainStop>();
            _timetableMap = new int[8];
        }


        private TrainTimetable ReadImpl()
        {
            string line;
            var lineIndex = 1;

            var currentTimetable = new List<TrainTime>();
            string currentTitle = null;
            var currentWeekDay = new List<TrainTimetableDay>() { TrainTimetableDay.Sunday, TrainTimetableDay.Monday, TrainTimetableDay.Tuesday, TrainTimetableDay.Wednesday, TrainTimetableDay.Thursday, TrainTimetableDay.Friday, TrainTimetableDay.Saturday, TrainTimetableDay.Holiday };
            //var currentAnnotations = new Dictionary<char, TrainAnnotation>();

            while ((line =_reader.ReadLine()) != null)
            {
                if (IsCommentLine(line))
                {
                    lineIndex++;
                    continue;
                }

                var trimedLine = line.TrimStart();

                var startChar = trimedLine[0];
                if (startChar == '#')
                {
                    currentTitle = ReadTitle(trimedLine);
                }
                else if (startChar == '$')
                {
                    _stops.Add(ReadStop(lineIndex, line));
                }
                else if (startChar == '[')
                {
                    FinallyCurrentTimetable(currentTitle, currentWeekDay, currentTimetable);
                    currentWeekDay = ReadWeekOfDay(lineIndex, line);
                    currentTimetable = new List<TrainTime>();
                    //_annotations = new Dictionary<char, TrainAnnotation>();
                    //currentAnnotations = new Dictionary<char, TrainAnnotation>();
                }
                else if (startChar >= '0' && startChar <= '9')
                {
                    ReadTimetable(lineIndex, trimedLine, currentTimetable);
                }
                else
                {
                    var anno = ReadAnnotation(lineIndex, trimedLine);

                    if (_annotations.ContainsKey(anno.Label))
                    {
                        _annotations[anno.Label] = anno;
                    }
                    else
                    {
                        _annotations.Add(anno.Label, anno);
                    }
                }
                lineIndex++;
            }
            FinallyCurrentTimetable(currentTitle, currentWeekDay, currentTimetable);


            _reader.Dispose();

            var result = new TrainTimetable();
            result.Stops = _stops;

            for (int i = 0; i < _timetableMap.Length; i++)
            {
                var table = _timetables[_timetableMap[i]];
                //table.Annotations = _annotations;
                result[(TrainTimetableDay)i] = table;
            }

            return result;
        }

        private bool IsCommentLine(string line)
        {
            return string.IsNullOrWhiteSpace(line) || line.StartsWith(";");
        }

        private string ReadTitle(string line)
        {
            var title = line.Substring(1);
            return title;
        }

        private void FinallyCurrentTimetable(string title, List<TrainTimetableDay> days, List<TrainTime> list)
        {
            if (list == null || list.Count == 0)
                return;

            var refIndex = _timetables.Count;
            list.Sort();

            foreach(var tt in list)
            {
                foreach(var key in tt.Annotations.Keys)
                {
                    tt.Annotations[key]= _annotations[key];
                }
            }

            var table = new TrainTimeList(list);

            var page = new TrainTimetablePage() { TimeList = table, Title = title };
            _timetables.Add(page);

            if (days == null || days.Count == 0)
                return;

            foreach(var day in days)
            {
                _timetableMap[(int)day] = refIndex;
            }

        }

        private List<TrainTimetableDay> ReadWeekOfDay(int lineIndex, string line)
        {
            var splits = line.Replace("[", "").Split(']');

            var days = new List<TrainTimetableDay>(splits.Length);   
            foreach(var day in splits)
            {
                days.Add(GetTimetableDayFromString(day));
            }
            return days;
        }

        private TrainTimetableDay GetTimetableDayFromString(string str)
        {
            switch (str.ToUpper())
            {
                case "SUN":
                    return TrainTimetableDay.Sunday;
                case "MON":
                    return TrainTimetableDay.Monday;
                case "TUE":
                    return TrainTimetableDay.Tuesday;
                case "WED":
                    return TrainTimetableDay.Wednesday;
                case "THU":
                    return TrainTimetableDay.Thursday;
                case "FRI":
                    return TrainTimetableDay.Friday;
                case "SAT":
                    return TrainTimetableDay.Saturday;
                case "HOL":
                    return TrainTimetableDay.Holiday;
            }
            return TrainTimetableDay.Holiday;
        }


        private void ReadTimetable(int lineIndex, string line, List<TrainTime> list)
        {
            var splits = line.Split(':');

            if (splits.Length != 2)
                return;


            if (!int.TryParse(splits[0], out var hour) || !(hour >= 0 && hour <= 23))
            {
                throw new FormatException($"Cannot parse hour text or invalid time at line {lineIndex}.");
            }

            var times = splits[1].Split(null);

            foreach(var timeText in times)
            {
                var text = timeText.Trim();
                if (string.IsNullOrEmpty(text))
                    continue;
               
                list.Add(ParseTime(lineIndex, hour, text));
            }

        }

        private TrainTime ParseTime(int lineIndex, int hour, string timeText)
        {
            string minuteText = null;
            var labels = new HashSet<char>();
            bool pass = false;
            for (int i = 0; i < timeText.Length; i++)
            {
                if(timeText[i] >= '0' && timeText[i] <= '9')
                {
                    minuteText = timeText.Substring(i);
                    break;
                }
                if (timeText[i] == '=')
                {
                    pass = true;
                    break;
                }

                if (!labels.Contains(timeText[i]))
                    labels.Add(timeText[i]);
            }


            var trainTime = new TrainTime();
            trainTime.Hour = hour;

            if (!pass)
            {
                if (string.IsNullOrEmpty(minuteText) || !int.TryParse(minuteText, out var minute) || !(minute >= 0 && minute <= 59))
                {
                    throw new FormatException($"Cannot parse minute text or invalid minute at line {lineIndex}.");
                }
                trainTime.Minute = minute;

            }
            else
            {
                trainTime.Minute = -1;
            }

            foreach (var label in labels)
                trainTime.AddAnnotation(label, null);

            return trainTime;
        }


        private TrainAnnotation ReadAnnotation(int lineIndex, string line)
        {
            string comment = null;
            var commentIndex = line.IndexOf(';');
            if (commentIndex != -1)
            {
                comment = line.Substring(commentIndex + 1);
                line = line.Substring(0, commentIndex);
            }

            var colonIndex = line.IndexOf(':');

            if (colonIndex == -1)
            {
                throw new FormatException($"Cannot parse annotation at line {lineIndex}.");
            }

            if (colonIndex != 1)
            {
                throw new FormatException($"Annotation label is not 1 character at line {lineIndex}.");
            }

            var label = line[0];

            var content = line.Substring(colonIndex + 1).TrimStart();
            return new TrainAnnotation(label, content, comment);
        }

        private TrainStop ReadStop(int lineIndex, string line)
        {
            var splits = line.Split(':');

            if (splits.Length != 2)
            {
                throw new FormatException($"Invalid stop format at line {lineIndex}.");
            }

            if(splits[0].Length <= 1)
            {
                throw new FormatException($"Invalid stop name at line {lineIndex}.");
            }

            var title = splits[0].Substring(1).Trim();

            var times = splits[1].Trim().Split(null);

            var result = new TrainStop() { Name = title };

            foreach (var timeText in times)
            {
                if (string.IsNullOrEmpty(timeText))
                    continue;

                if(timeText.StartsWith(">"))
                {
                    if(timeText.StartsWith(">/"))
                    {
                        result.TransferCfgFile = timeText.Substring(2);
                    }
                    else
                    {
                        result.TransferTblFile = timeText.Substring(1);
                    }
                    continue;
                }
                else if (timeText.StartsWith("+"))
                {
                    if(int.TryParse(timeText.Substring(1), out var t))
                    {
                        result.TransferTime = t;
                    }
                    continue;
                }

                var minute = ParseTime(lineIndex, 0, timeText);

                if(minute.Annotations.Count > 1)
                {
                    throw new FormatException($"Invalid minute at line {lineIndex}.");
                }

                if(minute.Annotations.Count == 0)
                {
                    result.AddDuration(minute.Minute);
                }
                else
                {
                    char key = minute.Annotations.Keys.ToArray()[0];
                    result.AddDuration(key, minute.Minute);
                }

            }
            return result;
        }


        public static TrainTimetable Read(TextReader reader)
        {
            var tblReader = new TblReader(reader);
            return tblReader.ReadImpl();
        }
        
    }

}
