namespace E_Journal.WebUI.Models.ViewModels;

public class PaginingViewModel
{
    public int ItemsCount { get; set; }
    public int ItemsPerPage { get; set; }
    public int CurrentPage { get; set; }
    public int PagesCount =>
        (int)Math.Ceiling((decimal)ItemsCount / ItemsPerPage);
}
