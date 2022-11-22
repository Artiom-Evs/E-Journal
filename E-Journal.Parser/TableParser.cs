using HtmlAgilityPack;
using E_Journal.Parser.Models;

namespace E_Journal.Parser;

/// <summary>
/// Contains methods for parsing schedule tables
/// </summary>
public static class TableParser
{
    /// <summary>
    /// Parse schedule table and returns table cells from it by dates
    /// </summary>
    public static IEnumerable<IEnumerable<PreparsedCell>> ParseTable(PreparsedTable preparsedTable)
    {
        // check empty table
        if (preparsedTable.RowsCells?.Any() != true) yield break;

        // take number of first lesson 
        int firstLessonNumber = GetFirstLessonNumber(preparsedTable.RowsCells.First().First());

        // take table cells from each table row
        var tableCells = preparsedTable.RowsCells
            .Select(rowCells => rowCells.Skip(1).ToArray())
            .ToArray();

        // get preparsed cells from grid of table cells
        var preparsedCells = BuildPreparsedCells(tableCells, preparsedTable.ColumnsDates, preparsedTable.ColumnsTitles, firstLessonNumber);

        foreach (var daylyCells in preparsedCells)
        {
            yield return daylyCells;
        }
    }

    /// <summary>
    /// Get first lesson number from first table row
    /// </summary>
    /// <exception cref="InvalidCastException">Throws when execution failed</exception>
    private static int GetFirstLessonNumber(HtmlNode firstTableRow)
    {
        // take text from first cell of table row
        string firstNumberCellText = firstTableRow.InnerText.Trim();

        if (firstNumberCellText.Length == 1 &&
            int.TryParse(firstNumberCellText, out int firstLessonNumber))
        {
            return firstLessonNumber;
        }

        throw new InvalidCastException("Failed determine the first lesson number.");
    }

    /// <summary>
    /// Groups lesson cells and room cells and returns them by dates
    /// </summary>
    private static IEnumerable<IEnumerable<PreparsedCell>> BuildPreparsedCells(HtmlNode[][] tableCells, DateTime[] studyDates, string[] scheduleHeaders, int firstLessonNumber)
    {
        for (int day = 0; day < studyDates.Length; day++)
        {
            List<PreparsedCell> dayCells = new();

            for (int row = 0; row < tableCells.Length; row++)
            {
                if (string.IsNullOrWhiteSpace(tableCells[row][day * 2].InnerText)) continue;

                dayCells.Add(new PreparsedCell()
                {
                    LessonCell = tableCells[row][day * 2],
                    RoomCell = tableCells[row][day * 2 + 1],
                    LessonDate = studyDates[day],
                    ScheduleHeader = scheduleHeaders[day],
                    LessonNumber = firstLessonNumber + row
                });
            }

            yield return dayCells;
        }
    }

    /// <summary>
    /// Get target table cells from table row. 
    /// </summary>
    private static IEnumerable<HtmlNode> GetRowCells(HtmlNode row)
    {
        var rowCells = row.Elements("td");
        string firstCellContent = row.Element("td")
            .InnerText
            .Replace("&nbsp;", "")
            .Trim();

        // this condition required for skip possible empty first column
        if (string.IsNullOrWhiteSpace(firstCellContent))
        {
            return rowCells.Skip(2);
        }

        return rowCells.Skip(1);
    }
}
