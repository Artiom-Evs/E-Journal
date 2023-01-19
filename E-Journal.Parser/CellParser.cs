using HtmlAgilityPack;
using E_Journal.Parser.Models;
using System.Text.RegularExpressions;
using System.Reflection.Metadata;

namespace E_Journal.Parser;

/// <summary>
/// Contains methods for parsing schedule table cells
/// </summary>
public static class CellParser
{
    public const string EmptyParameter = "unknown";

    /// <summary>
    /// Parse enumerable of schedule cells and return lessons from them
    /// </summary>
    public static IEnumerable<Lesson> ParseCells(IEnumerable<PreparsedCell> preparsedCells)
    {
        foreach (var cell in preparsedCells)
        {
            foreach (var lesson in ParseCell(cell))
            {
                yield return lesson;
            }
        }
    }

    /// <summary>
    /// Parse single schedule cell and return lessons from it
    /// </summary>
    public static IEnumerable<Lesson> ParseCell(PreparsedCell cell)
    {
        string[] lessonsCellRows = SplitCell(cell.LessonCell).ToArray();
        string[] roomsCellRows = SplitCell(cell.RoomCell).ToArray();

        // return empty enumerable if cell is empty
        if (lessonsCellRows.Length == 0)
        {
            yield break;
        }

        // check subgroup numeration
        // if false, build single lesson
        // if true, split by subgroup numbers and build lessons from each group of strings
        if (lessonsCellRows[0][1] != '.' ||
            !int.TryParse(lessonsCellRows[0][0..1], out _))
        {
            yield return BuildLesson(lessonsCellRows, roomsCellRows.FirstOrDefault() ?? "-", cell.ScheduleHeader, cell.LessonDate, cell.LessonNumber);
        }
        else
        {
            List<List<string>> lessons = new();
            List<string> lesson = new();

            // split all rows by subgroup numbers
            foreach (var rows in lessonsCellRows)
            {
                if (rows[1] == '.' &&
                    int.TryParse(rows[0..1], out _))
                {
                    lesson = new List<string>();
                    lessons.Add(lesson);
                }

                lesson.Add(rows);
            }

            foreach (var (lessonRows, room) in lessons.Zip(roomsCellRows))
            {
                yield return BuildLesson(lessonRows.ToArray(), room, cell.ScheduleHeader, cell.LessonDate, cell.LessonNumber);
            }
        }
    }

    /// <summary>
    /// Split string from lesson cell into strings from lesson cell
    /// </summary>
    /// <param name="cell">One cell of the schedule table</param>
    public static IEnumerable<string> SplitCell(HtmlNode cell)
    {
        // check numbered list
        // it can contain one or more elements, or one shifted element 
        // if true, table cell converted to the standard string format
        if (cell.Element("ol") != null)
        {
            // take ordered list
            var ol = cell.Element("ol");
            // take start value of the ordered list
            int startNumber = int.Parse(ol.Attributes["start"]?.Value ?? "1");

            // set subgroup numbers for each subgroup lesson and split them
            var strs = ol
                .Elements("li")
                .Select(li => li.InnerHtml.Replace("<p>", "").Replace("</p>", "").Replace("<br>", "\r\n").Trim())
                .Select(t => $"{startNumber++}.{t}")
                .Select(t => t.Split("\r\n", StringSplitOptions.TrimEntries));

            foreach (var str in strs)
            {
                foreach (var text in str)
                {
                    if (text.Equals("&nbsp;") || string.IsNullOrEmpty(text)) continue;
                    yield return text;
                }
            }
        }
        else
        {
            // split strings from lesson cell
            var result = cell.Elements("#text")
                .Select(t => t.InnerText.Trim())
                .Where(t => !t.Equals("&nbsp;"));

            foreach (var str in result)
            {
                yield return str;
            }
        }
    }

    /// <summary>
    /// Build lesson object based on data from lesson cell
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public static Lesson BuildLesson(string[] rows, string room, string scheduleHeader, DateTime date, int lessonNumber)
    {
        if (rows.Length == 0)
        {
            throw new ArgumentException("Count of rows cannot be equal zero.", nameof(rows));
        }

        if (string.IsNullOrEmpty(room))
        {
            throw new ArgumentException("String cannot be empty.", nameof(room));
        }

        if (lessonNumber <= 0)
        {
            throw new ArgumentException("Value cannot be less or equal than zero.", nameof(lessonNumber));
        }

        if (string.IsNullOrWhiteSpace(scheduleHeader))
        {
            throw new ArgumentException("String cannot be empty.", nameof(scheduleHeader));
        }

        if (date == default)
        {
            throw new ArgumentException("Date cannot be default.", nameof(date));
        }

        int subgroup = 0;

        // handle subgroup number
        if (rows[0][1] == '.'
            && int.TryParse(rows[0][0..1], out subgroup))
        {
            rows[0] = rows[0][2..].TrimStart();
        }

        // standard lessons
        if (rows.Length >= 3)
        {
            string title = rows[0];
            string type = rows[1].Replace("(", "").Replace(")", "");
            string subtitle = rows[2];

            return new Lesson()
            {
                Date = date,
                Title = title,
                Type = type,
                TeacherName = subtitle,
                GroupName = scheduleHeader,
                Room = room,
                Number = lessonNumber,
                Subgroup = subgroup
            };
        }
        
        if (subgroup != 0)
        {
            rows[0] = $"{subgroup}.{rows[0]}";
        }

        return BuildUncorrectLesson(rows, room, scheduleHeader, date, lessonNumber);
    }

    /// <summary>
    /// Build lesson object based uncorrect on data from lesson cell
    /// </summary>
    /// <param name="rows">Rows from lesson cell</param>
    /// <param name="room">Lesson room row</param>
    /// <exception cref="ArgumentException"></exception>
    public static Lesson BuildUncorrectLesson(string[] rows, string room, string scheduleHeader, DateTime date, int lessonNumber)
    {
        string title = EmptyParameter;
        string type = EmptyParameter;
        string subtitle = EmptyParameter;
        int subgroup = 0;


        if (rows.Length == 0)
        {
            throw new ArgumentException("Count of rows cannot be equal zero.", nameof(rows));
        }

        if (string.IsNullOrEmpty(room))
        {
            throw new ArgumentException("String cannot be empty.", nameof(room));
        }

        if (lessonNumber <= 0)
        {
            throw new ArgumentException("Value cannot be less or equal than zero.", nameof(lessonNumber));
        }

        if (string.IsNullOrWhiteSpace(scheduleHeader))
        {
            throw new ArgumentException("String cannot be empty.", nameof(scheduleHeader));
        }

        if (date == default)
        {
            throw new ArgumentException("Date cannot be default.", nameof(date));
        }

        // handle subgroup number
        if (rows[0][1] == '.'
            && int.TryParse(rows[0][0..1], out subgroup))
        {
            rows[0] = rows[0][2..].TrimStart();
        }

        // replace uncorrect room row information
        if (room.Contains("час"))
        {
            room = "-";
        }

        // handle lessons that contain two rows
        if (rows.Length == 2)
        {
            // if first row contains lesson type
            if (rows[0][^1] == ')')
            {
                title = rows[0][..(rows[0].LastIndexOf('('))];
                type = rows[0][(rows[0].LastIndexOf('(') + 1)..(rows[0].Length - 1)];
                subtitle = rows[1];
            }
            // if second row contains lesson type
            else if (rows[1][0] == '(')
            {
                title = rows[0];
                type = rows[1][1..(rows[1].IndexOf(')'))];
                
                if (rows[1][^1] != ')')
                {
                    subtitle = rows[1][(rows[1].IndexOf(')') + 1)..];
                }
            }
            // if both rows not contain lesson type
            else
            {
                title = rows[0];
                subtitle = rows[1];
            }
        }
        // handle lessons that contain single row
        else
        {
            // if single row contains lesson type
            if (rows[0].Contains('(') &&
                rows[0].Contains(')'))
            {
                string[] newRows = rows[0]
                    .Replace("(", "\r\n(")
                    .Replace(")", ")\r\n")
                    .Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

                if (subgroup != 0)
                {
                    newRows[0] = $"{subgroup}.{newRows[0]}";
                }

                return BuildLesson(newRows, room, scheduleHeader, date, lessonNumber);
            }
            // if single row contain teacher initials
            else if (rows[0][^1] == '.')
            {
                Regex reg = new(@"[А-Я][а-я]+\s?[А-Я]\.\s?[А-Я]\.$");

                subtitle = reg.Match(rows[0]).Value;

                if (subtitle != "" && subtitle != rows[0])
                {
                    title = rows[0][..(rows[0].IndexOf(subtitle))];
                }
            }
            // if single row contains only lesson title
            else
            {
                title = rows[0];
            }
        }

        return new Lesson()
        {
            Date = date,
            Title = title,
            Type = type,
            TeacherName = subtitle,
            GroupName = scheduleHeader,
            Room = room,
            Number = lessonNumber,
            Subgroup = subgroup
        };
    }
}
