using E_Journal.Shared;
using System.Diagnostics.CodeAnalysis;

internal class LessonEqualityComparer : IEqualityComparer<Lesson>
{
    public bool Equals(Lesson? x, Lesson? y)
    {
        if (x == null && y == null)
        {
            return true;
        }
        else if (x == null || y == null)
        {
            return false;
        }
        else if (x.GroupId == y.GroupId
            && x.Date == y.Date
            && x.Number == y.Number
            && x.Room == y.Room)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetHashCode([DisallowNull] Lesson obj)
    {
        long hCode = obj.GroupId
            ^ obj.Date.Ticks
            ^ obj.Number
            // TODO: разобраться с обновлением занятий подгрупп
            // ^obj.Subgroup
            ^ obj.Room.GetHashCode();

        return hCode.GetHashCode();
    }
}
