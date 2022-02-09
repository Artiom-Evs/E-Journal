using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;


namespace E_Journal.Parser
{
    public static class TimetableParser
    {
        public static ParseResult[] ParseTimetable(string pageText)
        {
            var document = ConvertToHtmlNode(pageText);
            var contentNode = CutContentNode(document);
            var nodeGroups = DivideToNodeGroups(contentNode).ToList();

            return nodeGroups.Select(group => ParseNodeGroup(group)).ToArray();
        }

        private static HtmlNode ConvertToHtmlNode(string textPage)
        {
            HtmlDocument doc = new();
            doc.LoadHtml(textPage);
            return doc.DocumentNode;
        }
        private static HtmlNode CutContentNode(HtmlNode document)
        {
            return document.SelectSingleNode("//div[@class='col-sm-9 content']//div");
        }
        private static IEnumerable<HtmlNode[]> DivideToNodeGroups(HtmlNode contentNode)
        {
            List<HtmlNode> blockGroup = new();

            foreach (HtmlNode node in contentNode.ChildNodes.Where(n => n.Name == "h2" || n.Name == "h3" || n.Name == "table"))
            {
                if (blockGroup.Any() && node.Name == "h2")
                {
                    yield return blockGroup.ToArray();
                    blockGroup = new();
                }

                blockGroup.Add(node);
            }
        }

        private static ParseResult ParseNodeGroup(HtmlNode[] nodeGroup)
        {
            string? name = null;
            string? dateRange = null;
            DateTime[]? days = null;
            string[][]? grid = null;
            
            try
            {
                name = nodeGroup[0].InnerText.Trim().Replace("Группа - ", "").Replace("Преподаватель - ", "");
                dateRange = nodeGroup.FirstOrDefault(n => n.Name == "h3")?.InnerText.Trim();
                
                if (nodeGroup.Length == 3)
                {
                    (days, grid) = ParseTable(nodeGroup[2]);
                }
            }
            catch (Exception ex)
            {
                return new ParseResult()
                {
                    Name = name, 
                    DateRange = dateRange, 
                    Days = days, 
                    Timetable = grid, 
                    Exception = ex
                };
            }

            return new ParseResult()
            {
                Name = name,
                DateRange = dateRange,
                Days = days,
                Timetable = grid
            };
        }
        private static (DateTime[], string[][]?) ParseTable(HtmlNode table)
        {
            var tableRows = table.Element("tbody").Elements("tr");

            var days = ParseDaysRow(tableRows.First());

            if (tableRows.Count() <= 2) return (days, null);

            var gridNodes = ConvertToGridNodes(tableRows.Skip(2));
            var gridStrs = ConvertToGridStrs(gridNodes);

            return (days, gridStrs);
        }
        private static DateTime[] ParseDaysRow(HtmlNode daysRow)
        {
            List<DateTime> days = new();
            DateTime buffer;

            foreach (HtmlNode node in daysRow.Elements("td").Skip(1))
            {
                if (DateTime.TryParse(node.InnerText, out buffer))
                {
                    days.Add(buffer);
                }
            }

            return days.Any() ? days.ToArray() : throw new InvalidOperationException("Не удалось определить даты учебных дней.");
        }

        private static HtmlNode[][] ConvertToGridNodes(IEnumerable<HtmlNode> rows)
        {
            return rows.Select(row => row.Elements("td").Skip(1).ToArray()).ToArray();
        }
        private static string[][] ConvertToGridStrs(HtmlNode[][] gridNodes)
        {
            return gridNodes.Select(row => row.Select(cell => cell.InnerText.Trim()).ToArray()).ToArray();
        }
        public static string[][] CleanUpExscess(string[][] grid)
        {
            return grid.Select(row => row.Select(cell => cell.Replace("&nbsp;", "")).ToArray()).ToArray();
        }
    }
}
