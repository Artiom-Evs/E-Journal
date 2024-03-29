﻿using HtmlAgilityPack;

namespace E_Journal.Parser.Models;

public class PreparsedTable
{
    public string[] ColumnsTitles { get; set; } = Array.Empty<string>();
    public DateTime[] ColumnsDates { get; set; } = Array.Empty<DateTime>();
    public HtmlNode[][] RowsCells { get; set; } = Array.Empty<HtmlNode[]>();
}
