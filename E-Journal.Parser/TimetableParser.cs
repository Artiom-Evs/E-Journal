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
        private static (string NameHeader, string DateRangeHeader) tags;

        public static ParseResult[] ParseTimetable(string pageText)
        {
            var document = ConvertToHtmlNode(pageText);
            var contentNode = CutContentNode(document);

            tags = (DefineNameTag(contentNode), DefineDataRangeTag(contentNode));

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

            foreach (HtmlNode node in ContentNodeChildSelector(contentNode.ChildNodes))
            {
                if (blockGroup.Any() && (node.InnerText.Contains("Группа - ") || node.InnerText.Contains("Преподаватель - ")))
                {
                    yield return blockGroup.ToArray();
                    blockGroup = new();
                }

                blockGroup.Add(node);
            }

            yield return blockGroup.ToArray();
        }

        private static ParseResult ParseNodeGroup(HtmlNode[] nodeGroup)
        {
            string name = string.Empty;
            string? dateRange = null;
            DateTime[]? days = null;
            string[][]? grid = null;
            int hashCode = 0;

            try
            {
                name = nodeGroup[0].InnerText.Trim().Replace("Группа - ", "").Replace("Преподаватель - ", "");
                dateRange = nodeGroup[1].InnerText.Trim();

                if (nodeGroup.Length == 3)
                {
                    (days, grid) = ParseTable(nodeGroup[2]);
                    hashCode = nodeGroup[2].InnerText.GetHashCode();
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
                    HashCode = hashCode,
                    Exception = ex
                };
            }

            return new ParseResult()
            {
                Name = name,
                DateRange = dateRange,
                Days = days,
                Timetable = grid,
                HashCode = hashCode
            };
        }
        private static (DateTime[], string[][]?) ParseTable(HtmlNode table)
        {
            var tableRows = table.Element("tbody").Elements("tr");

            var days = ParseDaysRow(tableRows.First());

            if (tableRows.Count() <= 2) return (days, null);

            var gridNodes = ConvertToGridNodes(tableRows.Skip(2));
            var gridStrs = ConvertToGridStrs(gridNodes);
            var invertedGrid = InverseGrid(gridStrs);

            return (days, invertedGrid);
        }
        private static DateTime[] ParseDaysRow(HtmlNode daysRow)
        {
            List<DateTime> days = new();

            foreach (HtmlNode node in daysRow.Elements("td").Skip(1))
            {
                if (DateTime.TryParse(node.InnerText, out DateTime buffer))
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
            return gridNodes.Select(row => row.Select(cell => CleanUpCell(cell.InnerHtml)).ToArray()).ToArray();
        }

        public static string CleanUpCell(string cell)
        {
            return cell.Replace("&nbsp;", "").Replace("<p>", "").Replace("<br> ", "\n").Replace("</p>", "").Trim();
        }
        private static string[][] InverseGrid(string[][] grid)
        {
            string[][] newGrid = new string[grid[0].Length / 2][];

            for (int col = 0; col < grid[0].Length; col += 2)
            {
                newGrid[col / 2] = new string[grid.Length * 2];

                for (int row = 0; row < grid.Length; row++)
                {
                    newGrid[col / 2][row * 2] = grid[row][col];
                    newGrid[col / 2][row * 2 + 1] = grid[row][col + 1];
                }
            }

            return newGrid;
        }

        private static IEnumerable<HtmlNode> ContentNodeChildSelector(IEnumerable<HtmlNode> nodes) => nodes
            .Where(n => n.Name == "table" || ((n.Name == tags.NameHeader || n.Name == tags.DateRangeHeader) && n.InnerText.Contains(" - ")));

        private static string DefineNameTag(HtmlNode contentNode) =>
            contentNode
            .ChildNodes
            .FirstOrDefault(n => n.InnerText.Contains("Группа - ") || n.InnerText.Contains("Преподаватель - "))
            ?.Name ?? throw new InvalidOperationException("Не удалось определить тег названия группы/преподавателя.");
        private static string DefineDataRangeTag(HtmlNode contentNode)
        {
            foreach (var node in contentNode.ChildNodes)
            {
                string[] dates = node.InnerText.Split(" - ");

                if (dates.Length != 2) continue;

                if (DateTime.TryParse(dates[0], out _) && DateTime.TryParse(dates[1], out _))
                {
                    return node.Name;
                }
            }

            throw new InvalidOperationException("Не удалось определить тег диапазона дат.");
        }
    }
}
