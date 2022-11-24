using NUnit.Framework;

using E_Journal.Parser;
using E_Journal.Parser.Models;
using System.Collections;
using System.Collections.Generic;
using System;
using HtmlAgilityPack;
using System.Linq;

namespace E_Journal.Parser.Tests;

[TestFixture]
public  class CellParserTests
{
    [Test]
    [TestCaseSource("GetPreparsedCellsWithSingleLessons")]
    public void CanParseCellsWithSingleLessons((PreparsedCell, Lesson[]) data)
    {
        var lessons = CellParser.ParseCell(data.Item1);

        Assert.True(Enumerable.SequenceEqual(lessons, data.Item2));
    }

    [Test]
    [TestCaseSource("GetPreparsedCellsWithMultipleLessons")]
    public void CanParseCellsWithMultipleLessons((PreparsedCell, Lesson[]) data)
    {
        var lessons = CellParser.ParseCell(data.Item1);

        Assert.True(Enumerable.SequenceEqual(lessons, data.Item2));
    }

    [Test]
    [TestCaseSource("GetPreparsedCellsWithoutLessonType")]
    public void CanParseCellsWithoutLessonType((PreparsedCell, Lesson[]) data)
    {
        var lessons = CellParser.ParseCell(data.Item1);

        Assert.True(Enumerable.SequenceEqual(lessons, data.Item2));
    }

    [Test]
    [TestCaseSource("GetPreparsedCellsWithUncorrectLineBreaks")]
    public void CanParseCellsWithUncorrectLineBreaks((PreparsedCell, Lesson[]) data)
    {
        var lessons = CellParser.ParseCell(data.Item1);

        Assert.True(Enumerable.SequenceEqual(lessons, data.Item2));
    }

    public static IEnumerable<(PreparsedCell, Lesson[])> GetPreparsedCellsWithSingleLessons()
    {
        yield return (
            new PreparsedCell()
            {
                LessonCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>Уч прак проф<br> (Практика)<br> Северин А. В.</p>\r\n</td>\r\n</td>\r\n"), 
                RoomCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>Кн 1-109(к)</p>\r\n</td>\r\n"), 
                LessonDate = DateTime.Parse("24/11/2022"), 
                ScheduleHeader = "162*", 
                LessonNumber = 1
            },
            new Lesson[]
            {
                new Lesson()
                {
                    Date = DateTime.Parse("24/11/2022"),
                    GroupName = "162*",
                    Number = 1,
                    Room = "Кн 1-109(к)",
                    Subgroup = 0,
                    TeatherName = "Северин А. В.",
                    Title = "Уч прак проф",
                    Type = "Практика"
                }
            });

        yield return (
            new PreparsedCell()
            {
                LessonCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>ЭлИз<br> (Лек)<br> Харевская Е. Т.</p>\r\n</td>\r\n"),
                RoomCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>Кн.1-204 маст</p>\r\n</td>\r\n"),
                LessonDate = DateTime.Parse("24/11/2022"),
                ScheduleHeader = "163*",
                LessonNumber = 2
            },
            new Lesson[]
            {
                new Lesson()
                {
                    Date = DateTime.Parse("24/11/2022"),
                    GroupName = "163*",
                    Number = 2,
                    Room = "Кн.1-204 маст",
                    Subgroup = 0,
                    TeatherName = "Харевская Е. Т.",
                    Title = "ЭлИз",
                    Type = "Лек"
                }
            });
    }

    public static IEnumerable<(PreparsedCell, Lesson[])> GetPreparsedCellsWithMultipleLessons()
    {
        yield return (
            new PreparsedCell()
            {
                LessonCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>1.Автомат электр(ЛР)<br> Самарская Н. В.<br> 2.Электрооб.ПиГЗ(КП)<br> Шеметов И. В.</p>\r\n</td>\r\n"),
                RoomCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>Кн.1-319<br> Кн.1-006</p>\r\n</td>\r\n"),
                ScheduleHeader = "43",
                LessonDate = DateTime.Parse("24/11/2022"),
                LessonNumber = 1
            },
            new Lesson[]
            {
                new Lesson()
                {
                        Date = DateTime.Parse("24/11/2022"),
                        GroupName = "43",
                        Number = 1,
                        Room = "Кн.1-319",
                        Subgroup = 1,
                        TeatherName = "Самарская Н. В.",
                        Title = "Автомат электр",
                        Type = "ЛР"
                },
                new Lesson()
                {
                    Date = DateTime.Parse("24/11/2022"),
                    GroupName = "43",
                    Number = 1,
                    Room = "Кн.1-006",
                    Subgroup = 2,
                    TeatherName ="Шеметов И. В.",
                    Title = "Электрооб.ПиГЗ",
                    Type = "КП"
                }
            });

        yield return (
            new PreparsedCell()
            {
                LessonCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>1.Стил рус. яз.(ЛР)<br> Зайковская М. И.<br> 2.ТСУ(ЛР)<br> Шавейко А. А.</p>\r\n</td>\r\n"),
                RoomCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>Кн.1-317<br> Кн.1-325(к)</p>\r\n</td>\r\n"),
                ScheduleHeader = "7",
                LessonDate = DateTime.Parse("24/11/2022"),
                LessonNumber = 2
            },
            new Lesson[]
            {
                new Lesson()
                {
                    Date = DateTime.Parse("24/11/2022"),
                    GroupName = "7",
                    Number = 2,
                    Room = "Кн.1-317",
                    Subgroup = 1,
                    TeatherName = "Зайковская М. И.",
                    Title = "Стил рус. яз.",
                    Type = "ЛР"
                },
                new Lesson()
                {
                    Date = DateTime.Parse("24/11/2022"),
                    GroupName = "7",
                    Number = 2,
                    Room = "Кн.1-325(к)",
                    Subgroup = 2,
                    TeatherName = "Шавейко А. А.",
                    Title = "ТСУ",
                    Type = "ЛР"
                }
            });

        yield return (
            new PreparsedCell()
            {
                LessonCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>1.Рус Язык (проф)(ЛР)<br> Зайковская М. И.<br> 2.Нем. яз.(ЛР)<br> Колышкина Л. Н.</p>\r\n</td>\r\n"),
                RoomCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>Кн.1-317<br> Кн.1-206</p>\r\n</td>\r\n"),
                ScheduleHeader = "8",
                LessonDate = DateTime.Parse("24/11/2022"),
                LessonNumber = 4
            },
            new Lesson[]
            {
                new Lesson()
                {
                    Date = DateTime.Parse("24/11/2022"),
                    GroupName = "8",
                    Number = 4,
                    Room = "Кн.1-317",
                    Subgroup = 1,
                    TeatherName = "Зайковская М. И.",
                    Title = "Рус Язык (проф)",
                    Type = "ЛР",
                },
                new Lesson()
                {
                    Date = DateTime.Parse("24/11/2022"),
                    GroupName = "8",
                    Number = 4,
                    Room = "Кн.1-206",
                    Subgroup = 2,
                    TeatherName = "Колышкина Л. Н.",
                    Title = "Нем. яз.",
                    Type = "ЛР"
                }
            });
    }

    public static IEnumerable<(PreparsedCell, Lesson[])> GetPreparsedCellsWithoutLessonType()
    {
        yield return (
            new PreparsedCell()
            {
                LessonCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>Уч прак проф<br> Северин А. В.</p>\r\n</td>\r\n</td>\r\n"),
                RoomCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>Кн 1-109(к)</p>\r\n</td>\r\n"),
                LessonDate = DateTime.Parse("24/11/2022"),
                ScheduleHeader = "162*",
                LessonNumber = 1
            },
            new Lesson[]
            {
                new Lesson()
                {
                    Date = DateTime.Parse("24/11/2022"),
                    GroupName = "162*",
                    Number = 1,
                    Room = "Кн 1-109(к)",
                    Subgroup = 0,
                    TeatherName = "Северин А. В.",
                    Title = "Уч прак проф",
                    Type = "unknown"
                }
            });

        yield return (
            new PreparsedCell()
            {
                LessonCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>2.ЭлИз <br>Харевская Е. Т.</p>\r\n</td>\r\n"),
                RoomCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>Кн.1-204 маст</p>\r\n</td>\r\n"),
                LessonDate = DateTime.Parse("24/11/2022"),
                ScheduleHeader = "163*",
                LessonNumber = 2
            },
            new Lesson[]
            {
                new Lesson()
                {
                    Date = DateTime.Parse("24/11/2022"),
                    GroupName = "163*",
                    Number = 2,
                    Room = "Кн.1-204 маст",
                    Subgroup = 2,
                    TeatherName = "Харевская Е. Т.",
                    Title = "ЭлИз",
                    Type = "unknown"
                }
            });

        yield return (
            new PreparsedCell()
            {
                LessonCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>Автомат электрСамарская Н. В.</p>\r\n</td>\r\n"),
                RoomCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>Кн.1-319<br> Кн.1-006</p>\r\n</td>\r\n"),
                ScheduleHeader = "43",
                LessonDate = DateTime.Parse("24/11/2022"),
                LessonNumber = 1
            },
            new Lesson[]
            {
                new Lesson()
                {
                        Date = DateTime.Parse("24/11/2022"),
                        GroupName = "43",
                        Number = 1,
                        Room = "Кн.1-319",
                        Subgroup = 0,
                        TeatherName = "Самарская Н. В.",
                        Title = "Автомат электр",
                        Type = "unknown"
                },
            });

    }

    public static IEnumerable<(PreparsedCell, Lesson[])> GetPreparsedCellsWithUncorrectLineBreaks()
    {
        yield return (
            new PreparsedCell()
            {
                LessonCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>Уч прак проф(Практика)<br> Северин А. В.</p>\r\n</td>\r\n</td>\r\n"),
                RoomCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>Кн 1-109(к)</p>\r\n</td>\r\n"),
                LessonDate = DateTime.Parse("24/11/2022"),
                ScheduleHeader = "162*",
                LessonNumber = 1
            },
            new Lesson[]
            {
                new Lesson()
                {
                    Date = DateTime.Parse("24/11/2022"),
                    GroupName = "162*",
                    Number = 1,
                    Room = "Кн 1-109(к)",
                    Subgroup = 0,
                    TeatherName = "Северин А. В.",
                    Title = "Уч прак проф",
                    Type = "Практика"
                }
            });

        yield return (
            new PreparsedCell()
            {
                LessonCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>2.ЭлИз <br>(Лек)Харевская Е. Т.</p>\r\n</td>\r\n"),
                RoomCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>Кн.1-204 маст</p>\r\n</td>\r\n"),
                LessonDate = DateTime.Parse("24/11/2022"),
                ScheduleHeader = "163*",
                LessonNumber = 2
            },
            new Lesson[]
            {
                new Lesson()
                {
                    Date = DateTime.Parse("24/11/2022"),
                    GroupName = "163*",
                    Number = 2,
                    Room = "Кн.1-204 маст",
                    Subgroup = 2,
                    TeatherName = "Харевская Е. Т.",
                    Title = "ЭлИз",
                    Type = "Лек"
                }
            });

        yield return (
            new PreparsedCell()
            {
                LessonCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>Автомат электр(ЛР)Самарская Н. В.</p>\r\n</td>\r\n"),
                RoomCell = HtmlNode.CreateNode("\r\n<td>\r\n<p>Кн.1-319<br> Кн.1-006</p>\r\n</td>\r\n"),
                ScheduleHeader = "43",
                LessonDate = DateTime.Parse("24/11/2022"),
                LessonNumber = 1
            },
            new Lesson[]
            {
                new Lesson()
                {
                        Date = DateTime.Parse("24/11/2022"),
                        GroupName = "43",
                        Number = 1,
                        Room = "Кн.1-319",
                        Subgroup = 0,
                        TeatherName = "Самарская Н. В.",
                        Title = "Автомат электр",
                        Type = "ЛР"
                }
            });

    }
}
