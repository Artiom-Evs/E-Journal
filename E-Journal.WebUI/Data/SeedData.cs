using E_Journal.Shared;
using E_Journal.Infrastructure;

namespace E_Journal.WebUI.Data;
public static class SeedData
{
    private static readonly ScoreValue[] scoreValues = 
        new ScoreValue[]
        {
            new() { Title = "1" },
            new() { Title = "2" },
            new() { Title = "3" },
            new() { Title = "4" },
            new() { Title = "5" },
            new() { Title = "6" },
            new() { Title = "7" },
            new() { Title = "8" },
            new() { Title = "9" },
            new() { Title = "10" },
            new() { Title = "Зачёт" },
            new() { Title = "Незачёт" },
        };

    public static void EnsureScoreValuesCreation(IApplicationBuilder app)
    {
        JournalDbContext context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<JournalDbContext>();

        foreach (var scoreValue in scoreValues)
        {
            bool hasEntity = context.ScoreValues.Any(v => v.Title == scoreValue.Title);

            if (!hasEntity)
            {
                context.ScoreValues.Add(scoreValue);
            }
        }

        context.SaveChanges();
    }
}
