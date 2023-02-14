using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using System.Reflection;

namespace E_Journal.Identity.Data;

public sealed class Roles
{
    private Roles() { }

    public const string Admin = "Администратор";
    public const string Teacher = "Преподаватель";
    public const string UnconfirmedTeacher = "Неподтверждённый преподаватель";
    public const string Student = "Учащийся";
    public const string UnconfirmedStudent = "Неподтверждённый учащийся";

    public static string[] GetAll()
    {
        Roles rolesObj = new();
        List<string> constants = new();
        FieldInfo[] fieldInfos = typeof(Roles).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        foreach (FieldInfo fi in fieldInfos)
            if (fi.IsLiteral && !fi.IsInitOnly)
                constants.Add((string?)fi.GetValue(rolesObj) ?? throw new InvalidOperationException());

        return constants.ToArray();
    }
}
