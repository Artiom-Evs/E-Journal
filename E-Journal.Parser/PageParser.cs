using HtmlAgilityPack;
using E_Journal.Parser.Models;
using System.Globalization;

namespace E_Journal.Parser;

/// <summary>
/// Contains methods for parsing schedule pages
/// </summary>
public static class PageParser
{
    /// <summary>
    /// Parse weekly schedules from weekly page string
    /// </summary>
    public static IEnumerable<PreparsedTable> ParseWeeklySchedules(string pageText)
    {
        var document = BuildRootPageNode(pageText);
        var contentNode = GetPageContentNode(document);

        var nodeGroups = GetTitleAndTableGroupsFromPageContent(contentNode);
        var preparsedTables = nodeGroups.Select(g => BuildWeeklyPreparsedTable(g.Item1, g.Item2));

        return preparsedTables;
    }

    /// <summary>
    /// Parse dayly schedules from dayly page string
    /// </summary>
    public static IEnumerable<PreparsedTable> ParseDaylySchedules(string pageText)
    {
        var document = BuildRootPageNode(pageText);
        var contentNode = GetPageContentNode(document);

        var tables = contentNode.Elements("table");

        // take target rows from all dayly schedule tables
        List<HtmlNode> tableRows = new();
        tables.ToList()
            .ForEach(t => tableRows.AddRange(GetTargetRowsOfDaylyTable(t)));

        // split all dayly schedule tables rows into blocks of schedule
        var tableBlocks = SplitDaylyTableRowsByTableBlocks(tableRows);

        var preparsedTables = tableBlocks.Select(t => BuildDaylyPreparsedTable(t));

        return preparsedTables;
    }

    /// <summary>
    /// Build rootHtmlNode of html page
    /// </summary>
    /// <param name="textPage"></param>
    /// <returns></returns>
    private static HtmlNode BuildRootPageNode(string textPage)
    {
        HtmlDocument doc = new();
        doc.LoadHtml(textPage);
        return doc.DocumentNode;
    }

    /// <summary>
    /// Get page content node from root page node
    /// </summary>
    private static HtmlNode GetPageContentNode(HtmlNode document)
    {
        return document.SelectSingleNode("//div[@id='main-p']//div").Element("div");
    }

    /// <summary>
    /// Get target rows of the dayly schedule table
    /// </summary>
    private static IEnumerable<HtmlNode> GetTargetRowsOfDaylyTable(HtmlNode tableNode) =>
        tableNode
            .Element("tbody")
            .Elements("tr")
            .Where(tr =>
                TryParseDaylyTableLessonRow(tr, out _) ||
                TryParseDaylyTableTitlesRow(tr, out _) ||
                TryParseDaylyTableDateRow(tr, out _));

    /// <summary>
    /// Split single dayly schedule table by dates rows to blocks into dayly schedules
    /// </summary>
    /// <param name="tableRows">Rows of single dayly schedule table</param>
    private static IEnumerable<HtmlNode[]> SplitDaylyTableRowsByTableBlocks(IEnumerable<HtmlNode> tableRows)
    {
        List<HtmlNode> groupsBlock = new();

        foreach (var row in tableRows)
        {
            var result = TryParseDaylyTableDateRow(row, out _);

            if (result)
            {
                if (groupsBlock.Count > 0)
                    yield return groupsBlock.ToArray();

                groupsBlock = new();
            }

            groupsBlock.Add(row);
        }

        yield return groupsBlock.ToArray();
    }

    /// <summary>
    /// Try to get the date from the date row from the weekly schedule table 
    /// </summary>
    /// <returns>Returns true if the operation was successful</returns>
    private static bool TryParseDaylyTableDateRow(HtmlNode tableRow, out DateTime result)
    {
        var node = tableRow
            .Elements("td")
            .FirstOrDefault(n => n.InnerText.Contains("День - "))
            ?.Element("p");

        if (node == null ||
            !DateTime.TryParse(node.InnerText[7..], new CultureInfo("ru-Ru"), DateTimeStyles.None, out result))
        {
            result = default;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Try to get the titles from the titles row from the weekly schedule table 
    /// </summary>
    /// <returns>Returns true if the operation was successful</returns>
    private static bool TryParseDaylyTableTitlesRow(HtmlNode tableRow, out string[]? result)
    {
        string rowText = tableRow.InnerText
            .Replace("&nbsp;", "")
            .Trim();

        if (rowText.ElementAtOrDefault(0) != '№')
        {
            result = null;
            return false;
        }

        result = rowText[1..]
            .Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
            .ToArray();

        return true;
    }

    /// <summary>
    /// Try to get the lesson row cells from the titles row from the weekly schedule table 
    /// </summary>
    /// <returns>Returns true if the operation was successful</returns>
    private static bool TryParseDaylyTableLessonRow(HtmlNode tableRow, out HtmlNode[]? result)
    {
        string rowText = tableRow.InnerText.Trim();
        result = tableRow.Elements("td").ToArray();

        if (rowText.StartsWith("&nbsp;"))
        {
            rowText = rowText[("&nbsp;".Length)..].TrimStart();
            result = result[1..];
        }

        // determine if a table row is a lesson row
        // lessons row starts always with a numeric character that defines the lesson number
        bool isLessonRow = result.Any() && char.IsNumber(rowText[0]) && !char.IsNumber(rowText[1]);

        if (!isLessonRow)
        {
            result = null;
            return false;
        }

        if (result.Count() % 2 == 0)
        {
            result = result[..^1];
        }

        return true;
    }

    /// <summary>
    /// Build preparsed table object based on rows of dayly schedule table
    /// </summary>
    private static PreparsedTable BuildDaylyPreparsedTable(HtmlNode[] tableRows)
    {
        // throw an exception if it was not possible to get date of dayly schedule
        if (!TryParseDaylyTableDateRow(tableRows[0], out DateTime tableDate)
            || tableDate == default)
        {
            throw new InvalidOperationException("Failed to get date of the dayly schedule");
        }

        // throw an exception if it was not possible to get groups titles
        if (!TryParseDaylyTableTitlesRow(tableRows[1], out string[]? titles)
            || titles == null)
        {
            throw new InvalidOperationException("Failed to get groups titles from the dayly schedule");
        }

        // create date array with date for each dayly schedule
        var dates = Enumerable.Range(0, titles.Length)
            .Select(i => tableDate)
            .ToArray();

        List<HtmlNode[]> rowsCells = new();

        foreach (var row in tableRows.Skip(2))
        {
            if (TryParseDaylyTableLessonRow(row, out HtmlNode[]? rowCells) && rowCells != null)
            {
                rowsCells.Add(rowCells);
            }
        }

        return new PreparsedTable()
        {
            ColumnsTitles = titles,
            ColumnsDates = dates,
            RowsCells = rowsCells.ToArray()
        };
    }

    /// <summary>
    /// Get enumerable of title and table groups from page content
    /// </summary>
    private static IEnumerable<(HtmlNode, HtmlNode?)> GetTitleAndTableGroupsFromPageContent(HtmlNode contentNode)
    {
        HtmlNode? title = null;
        HtmlNode? table = null;

        var scheduleTitleTag = GetTitleTagName(contentNode);
        var targetElements = contentNode.ChildNodes
            .Where(n => n.Name.Equals("table") ||
            n.Name.Equals(scheduleTitleTag) && (n.InnerText.Contains("Группа - ") || n.InnerText.Contains("Преподаватель - ")));

        foreach (HtmlNode node in targetElements)
        {
            if (node.Name.Equals(scheduleTitleTag))
            {
                if (title != null)
                {
                    yield return (title, table);

                    table = null;
                }

                title = node;
            }
            else if (node.Name.Equals("table"))
            {

                table = node;
            }
        }

        yield return (title, table);
    }

    /// <summary>
    /// Get tag name of HtmlNode, that contains title of schedule table
    /// </summary>
    /// <exception cref="InvalidOperationException">Throws if execution failed</exception>
    private static string GetTitleTagName(HtmlNode contentNode) =>
        contentNode
        .ChildNodes
        .FirstOrDefault(n => n.InnerText.Contains("Группа - ") || n.InnerText.Contains("Преподаватель - "))
        ?.Name ?? throw new InvalidOperationException("Failed to determine schedule title tag.");

    /// <summary>
    /// Build preparsed table object based on title node and table node
    /// </summary>
    private static PreparsedTable BuildWeeklyPreparsedTable(HtmlNode title, HtmlNode? table)
    {
        // returns empty preparsed table object with single title, if schedule do not have table
        if (table == null)
        {
            return new PreparsedTable()
            {
                ColumnsTitles = new[] { title.InnerText }
            };
        }

        var tableRows = table
            .Element("tbody")
            .Elements("tr");

        var columnsDates = ParseWeeklyScheduleDatesRow(tableRows.First());
        var columnsTitles = Enumerable.Range(0, columnsDates.Length)
            .Select(i => title.InnerText.Replace("Группа - ", "").Replace("Преподаватель - ", ""))
            .ToArray();

        var rowsCells = tableRows.Skip(2)
            .Select(tr => tr.SelectNodes("th | td").ToArray())
            .ToArray();

        return new PreparsedTable()
        {
            ColumnsTitles = columnsTitles,
            ColumnsDates = columnsDates,
            RowsCells = rowsCells
        };
    }

    /// <summary>
    /// Parse table row with dates from weekly schedule and returns those dates
    /// </summary>
    /// <exception cref="InvalidOperationException">Throws if no date is received</exception>
    private static DateTime[] ParseWeeklyScheduleDatesRow(HtmlNode daysRow)
    {
        List<DateTime> days = new();

        foreach (HtmlNode node in daysRow.Elements("th").Skip(1))
        {
            if (DateTime.TryParse(node.InnerText, new CultureInfo("ru-Ru"), DateTimeStyles.None, out DateTime buffer))
            {
                days.Add(buffer);
            }
        }

        return days.Any() ? days.ToArray() : throw new InvalidOperationException("Не удалось определить даты учебных дней.");
    }
}
