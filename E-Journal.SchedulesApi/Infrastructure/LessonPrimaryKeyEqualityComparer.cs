using System.Diagnostics.CodeAnalysis;
using E_Journal.SchedulesApi.Models;

namespace E_Journal.SchedulesApi.Infrastructure;

public class LessonPrimaryKeyEqualityComparer : IEqualityComparer<Lesson>
{
    public bool Equals(Lesson? x, Lesson? y)
    {
        if (x == y) return true;

        if (x == null || y == null) return false;

        bool result =
            x.Group.Name == y.Group.Name
            && x.Date == y.Date
            && x.Subgroup == y.Subgroup
            && x.Number == y.Number;

        return result;
    }

    public int GetHashCode([DisallowNull] Lesson obj)
    {
        string str = 
            obj.Group.Name +
            obj.Date.ToString() +
            obj.Number.ToString() +  
            obj.Subgroup.ToString();

        return str.GetHashCode();
    }
}
