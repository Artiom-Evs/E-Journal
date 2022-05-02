﻿#nullable disable
using E_Journal.Shared;

namespace E_Journal.WebUI.Models.ViewModels
{
    public record GroupScheduleViewModel
    {
        public string GroupName { get; init; }
        public DateTime[] Dates { get; init; }
        public LessonViewModel[][] Lessons { get; init; }
        public int MaxLessonsCount { get; init; }
    }
}