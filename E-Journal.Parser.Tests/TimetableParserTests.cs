using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

using E_Journal.Parser;

namespace E_Journal.Parser.Tests
{
    public class TimetableParserTests
    {
        private static readonly string oldPageText;
        private static readonly string newPageText;
        
        static TimetableParserTests()
        {
            oldPageText = System.IO.File.ReadAllText("timetable.html");
            newPageText = System.IO.File.ReadAllText("timetable_new.html");
        }

        public static IEnumerable<string> GetTextsOfPages()
        {
            yield return oldPageText;
            yield return oldPageText;
        }

        [Test]
        public void CanParseOldTimetables()
        {
            ParseResult[] result = TimetableParser.ParseTimetable(oldPageText);

            Assert.True(10 == result.Where(r => r.Timetable is not null).Count());
        }
        [Test]
        public void CanParseNewTimetables()
        {
            ParseResult[] result = TimetableParser.ParseTimetable(newPageText);

            Assert.True(11 == result.Where(r => r.Timetable is not null).Count());
        }

        [Test]
        public void CanSeparateOldTimetablePage()
        {
            ParseResult[] result = TimetableParser.ParseTimetable(oldPageText);

            Assert.True(13 == result.Length);
        }
        [Test]
        public void CanSeparateNewTimetablePage()
        {
            ParseResult[] result = TimetableParser.ParseTimetable(newPageText);

            Assert.True(11 == result.Length);
        }

        [Test]
        public void CanParseOldTableCells()
        {
            ParseResult[] result = TimetableParser.ParseTimetable(oldPageText);

            Assert.IsNotNull(result[5].Timetable);

            string[][] table = result[5].Timetable;

            Assert.AreEqual(6, table.Length);
            Assert.AreEqual(14, table[0].Length);
            Assert.AreEqual("Обществовед\n(Лек)\nЧертков М. Д.", table[0][0]);
            Assert.AreEqual("-", table[0][1]);
            Assert.AreEqual("1.ФЗКиЗ\n(Лек)\nАнципов Е. Ю.\n2.ФЗКиЗ\n(Лек)\nЖукова Т. Ю.", table[0][4]);
            Assert.AreEqual("-\n-", table[0][5]);
        }

        [Test]
        [TestCaseSource("GetTextsOfPages")]
        public void CanWork(string pageText)
        {
            ParseResult[] results = TimetableParser.ParseTimetable(pageText);

            foreach (var r in results)
            {
                Assert.IsNotEmpty(r.Name);
                Assert.IsNotEmpty(r.DateRange);
            }
        }
    }
}