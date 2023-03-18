using System.IO;
using Xunit;
using System.Text;

namespace NextTrainLib.Test
{
    public class TblReaderTest
    {
        [Fact]
        public void TesterTest()
        {
            Assert.True(true);
        }

        [Fact]
        public void LoadBasic()
        {
            var tblFile = @"
; comment
a: local
b: rapid;kaisoku

12: a0 b5 a10 b8
";
            TrainTimetable tbl;
            using (var sr = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(tblFile))))
            {
                tbl = TblReader.Read(sr);
            }

            Assert.NotNull(tbl);
            Assert.Equal(4, tbl.GetPage(TrainTimetableDay.Tuesday).TimeList.Count);
            Assert.Equal(12, tbl.GetPage(TrainTimetableDay.Tuesday).TimeList[0].Hour);
            Assert.Equal(5, tbl.GetPage(TrainTimetableDay.Tuesday).TimeList[1].Minute);
            Assert.Equal(8, tbl.GetPage(TrainTimetableDay.Tuesday).TimeList[2].Minute);
            Assert.Equal(10, tbl.GetPage(TrainTimetableDay.Tuesday).TimeList[3].Minute);


            Assert.Single(tbl.GetPage(TrainTimetableDay.Wednesday).TimeList[0].Annotations);
            Assert.Single(tbl.GetPage(TrainTimetableDay.Wednesday).TimeList[1].Annotations);

            Assert.True(tbl.GetPage(TrainTimetableDay.Wednesday).TimeList[0].Annotations.ContainsKey('a'));
            Assert.Equal('a', tbl.GetPage(TrainTimetableDay.Wednesday).TimeList[0].Annotations['a'].Label);
            Assert.Equal("local", tbl.GetPage(TrainTimetableDay.Wednesday).TimeList[0].Annotations['a'].Content);


            Assert.True(tbl.GetPage(TrainTimetableDay.Wednesday).TimeList[1].Annotations.ContainsKey('b'));
            Assert.Equal('b', tbl.GetPage(TrainTimetableDay.Wednesday).TimeList[1].Annotations['b'].Label);
            Assert.Equal("rapid", tbl.GetPage(TrainTimetableDay.Wednesday).TimeList[1].Annotations['b'].Content);
            Assert.Equal("kaisoku", tbl.GetPage(TrainTimetableDay.Wednesday).TimeList[1].Annotations['b'].Comment);
        }


        [Fact]
        public void LoadDayOfWeek()
        {
            var tblFile = @"
12: 0
[SAT][SUN]
12: 10
[HOL]
12: 20
";
            TrainTimetable tbl;
            using (var sr = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(tblFile))))
            {
                tbl = TblReader.Read(sr);
            }

            Assert.NotNull(tbl);
            Assert.Equal(1, tbl.GetPage(TrainTimetableDay.Saturday).TimeList.Count);
            Assert.Equal(10, tbl.GetPage(TrainTimetableDay.Saturday).TimeList[0].Minute);
            Assert.Equal(20, tbl.GetPage(TrainTimetableDay.Holiday).TimeList[0].Minute);
            Assert.Equal(0, tbl.GetPage(TrainTimetableDay.Monday).TimeList[0].Minute);
            Assert.Equal(0, tbl.GetPage(TrainTimetableDay.Tuesday).TimeList[0].Minute);
            Assert.Equal(0, tbl.GetPage(TrainTimetableDay.Wednesday).TimeList[0].Minute);
            Assert.Equal(0, tbl.GetPage(TrainTimetableDay.Thursday).TimeList[0].Minute);
            Assert.Equal(0, tbl.GetPage(TrainTimetableDay.Friday).TimeList[0].Minute);

        }

        [Fact]
        public void LoadStops()
        {
            var tblFile = @"
a:rapid
b:new rapid

$a: 10 a8 b6
$b:9 a7 b= c5
$c:2 a2 b2 c2
12: 0
";
            TrainTimetable tbl;
            using (var sr = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(tblFile))))
            {
                tbl = TblReader.Read(sr);
            }

            Assert.NotNull(tbl);
            Assert.Equal(1, tbl.GetPage(TrainTimetableDay.Saturday).TimeList.Count);
            Assert.Equal(0, tbl.GetPage(TrainTimetableDay.Saturday).TimeList[0].Minute);
            Assert.Equal(3, tbl.Stops.Count);
            Assert.Equal(10, tbl.Stops[0].GetDuration());
            Assert.Equal(8, tbl.Stops[0].GetDuration('a'));
            Assert.Equal(6, tbl.Stops[0].GetDuration('b'));
            Assert.False(tbl.Stops[1].IsTrainStop('b'));

        }

        [Fact]
        public void TrainTimeList()
        {
            var ttl = new TrainTimeList();
            ttl.Add(new TrainTime(5, 0));
            ttl.Add(new TrainTime(7, 0));
            ttl.Add(new TrainTime(11, 0));
            Assert.Equal(3, ttl.Count);

            ttl.Add(new TrainTime(6, 0));
            ttl.Add(new TrainTime(8, 0));
            Assert.Equal(5, ttl.Count);
            Assert.Equal(5, ttl[0].Hour);
            Assert.Equal(6, ttl[1].Hour);
            Assert.Equal(7, ttl[2].Hour);
            Assert.Equal(8, ttl[3].Hour);

            ttl.Add(new TrainTime(4, 0));
            Assert.Equal(4, ttl[0].Hour);

            ttl.RemoveAt(1);
            Assert.Equal(6, ttl[1].Hour);

            ttl.RemoveAt(0);
            Assert.Equal(7, ttl[1].Hour);

        }
    }

}
