using E_Journal.Parser.Models;

namespace E_Journal.SchedulesApi.Extensions;

public static class ErrorStringBuildExtensions
{
    public static string ToErrorString(this PreparsedCell cell)
    {
        return
            "Not parsed cell:" +
            $"\r\nSchedule header: {cell.ScheduleHeader}" + 
            $"\r\nLesson cell: {cell.LessonCell.OuterHtml.Trim()}" +
            $"\r\nRoom cell: {cell.RoomCell.OuterHtml.Trim()}" +
            $"\r\nLesson number: {cell.LessonNumber}" +
            $"\r\nLesson date: {cell.LessonDate}";
    }

    public static string ToErrorString(this PreparsedTable table)
    {
        string[] rows = table.RowsCells
            .Select(r => string.Join("\r\n\t", r
                .Select((c, i) => $"#{i}>> {c.OuterHtml.Trim()}")))
            .ToArray();

        string tableGrid = string.Join("\r\n", 
            rows.Select((r, i) => $"Column #{i}:\r\n\t{r}"));

        return
            "Not parsed table:" +
            $"\r\nSchedule column titles: [{string.Join(", ", table.ColumnsTitles)}]" +
            $"\r\nSchedule column dates: [{string.Join(", ", table.ColumnsDates)}]" +
            "\r\nRows:\r\n" +
            tableGrid;
    }
}
