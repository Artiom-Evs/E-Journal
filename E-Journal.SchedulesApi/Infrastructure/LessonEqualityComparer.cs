using System.Diagnostics.CodeAnalysis;
using E_Journal.SchedulesApi.Models;

namespace E_Journal.SchedulesApi.Infrastructure;

public class LessonEqualityComparer : IEqualityComparer<Lesson>
{
    public bool Equals(Lesson? x, Lesson? y)
    {
        if (x == y) return true;

        if (x == null || y == null) return false;

        bool result =
            x.Group.Name == y.Group.Name
            && x.Subject.Name == y.Subject.Name
            && x.Type.Name == y.Type.Name
            && x.Teacher.Name == y.Teacher.Name
            && x.Room.Name == y.Room.Name
            && x.Subgroup == y.Subgroup
            && x.Number == y.Number
            && x.Date == y.Date;

        return result;
    }

    public int GetHashCode([DisallowNull] Lesson obj)
    {
        return obj.ToString().GetHashCode();
    }
}
