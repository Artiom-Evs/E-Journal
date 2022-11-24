
using E_Journal.Parser.Models;
using HtmlAgilityPack;
using System.Text.Json;
using System;
using System.Linq;

namespace E_Journal.Parser.Tests;

internal static class ConvertationExtensions
{
    internal static string ToJson(this PreparsedCell cell)
    {
        var obj = new
        {
            LessonCell = cell.LessonCell.OuterHtml,
            RoomCell = cell.RoomCell.OuterHtml,
            Date = cell.LessonDate.ToString("dd.MM.yyyy"),
            ScheduleHeader = cell.ScheduleHeader,
            LessonNumber = cell.LessonNumber
        };

        return System.Text.Json.JsonSerializer.Serialize(obj);
    }
    internal static string ToJson(this PreparsedCell[][] nodes)
    {

        string[][] jsonCells = nodes
            .Select(pcarr => pcarr
            .Select(pc => pc.ToJson())
                .ToArray())
            .ToArray();

        return JsonSerializer.Serialize(jsonCells);
    }

    internal static string ToJson(this PreparsedTable table)
    {
        string[][] tableCells = table.RowsCells
            .Select(r => r
                .Select(c => c.OuterHtml).ToArray())
            .ToArray();

        var obj = new
        {
            RowsCells = tableCells,
            table.ColumnsTitles,
            table.ColumnsDates
        };
        
        return JsonSerializer.Serialize(obj);
    }
    internal static PreparsedTable ToPreparsedTable(this string jsonText)
    {
        JsonElement jRoot = JsonDocument.Parse(jsonText).RootElement;

        DateTime[] dates = jRoot.GetProperty("ColumnsDates")
            .EnumerateArray()
            .Select(d => DateTime.Parse(d.GetString() ?? throw new InvalidOperationException("Failuer to get date string from JSON")))
            .ToArray();

        string[] titles = jRoot.GetProperty("ColumnsTitles")
            .EnumerateArray()
            .Select(d => d.GetString() ?? throw new InvalidOperationException("Failuer to get string from JSON"))
            .ToArray();

        HtmlNode[][] cells = jRoot.GetProperty("RowsCells")
            .EnumerateArray()
            .Select(row =>
            row.EnumerateArray()
                .Select(cell => HtmlNode.CreateNode(cell.GetString() ?? throw new InvalidOperationException("Failuer to get HTML string from JSON")))
                .ToArray())
            .ToArray();

        return new PreparsedTable()
        {
            ColumnsTitles = titles,
            ColumnsDates = dates,
            RowsCells = cells
        };
    }
}
