// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace  E_Journal.WebUI.Areas.Admin.Pages.Accounts
{
    public static class ManageNavPages
    {
        public static string Index => "Index";
        public static string Students => "Students";
        public static string Teachers => "Teachers";
        public static string UnconfirmedUsers => "UnconfirmedUsers";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);
        public static string StudentsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Students);
        public static string TeachersNavClass(ViewContext viewContext) => PageNavClass(viewContext, Teachers);
        public static string UnconfirmedNavClass(ViewContext viewContext) => PageNavClass(viewContext, UnconfirmedUsers);

        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
