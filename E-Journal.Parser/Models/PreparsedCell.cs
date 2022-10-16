#nullable disable

using E_Journal.Parser.Models;
using HtmlAgilityPack;

namespace E_Journal.Parser.Models;

public class PreparsedCell
{
    internal PreparsedCell() { }

    public HtmlNode LessonCell { get; set; }
    public HtmlNode RoomCell { get; set; }
    public int LessonNumber { get; set; }
    public DateTime LessonDate { get; set; }
}
