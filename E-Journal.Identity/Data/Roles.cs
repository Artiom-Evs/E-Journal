using Microsoft.AspNetCore.Identity;

namespace E_Journal.Identity.Data;

public sealed class Roles
{
    private Roles() { }

    public static string Admin => "Администратор";
    public static string Teacher => "Преподаватель";
    public static string UnconfirmedTeacher => "Неподтверждённый преподаватель";
    public static string Student => "Учащийся";
    public static string UnconfirmedStudent => "Неподтверждённый учащийся";

    public static string[] GetAll()
    {
        Roles rolesObj = new();
        string[] roles = rolesObj.GetType().GetProperties()
            .Select(p => (string)(p.GetValue(rolesObj) ?? throw new InvalidOperationException("All properties of the Roles class should only return string.")))
            .ToArray();

        return roles;
    }
}
