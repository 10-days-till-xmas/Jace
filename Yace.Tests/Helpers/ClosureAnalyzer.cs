using System.Reflection;
using static Yace.Tests.Helpers.OutputHelper;
namespace Yace.Tests.Helpers;

public static class ClosureAnalyzer
{
    public static void PrintClosureInfo(object obj)
    {
        const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance
                                 | BindingFlags.Public | BindingFlags.Static;
        var type = obj.GetType();
        foreach (var member in type.GetMembers(flags))
            Output.WriteLine(member switch
            {
                FieldInfo field => $"Field: {field.Name}, Type: {field.FieldType}, Value: {field.GetValue(obj)}",
                PropertyInfo prop => $"Property: {prop.Name}, Type: {prop.PropertyType}, Value: {prop.GetValue(obj)}",
                MethodInfo method => $"Method: {method.Name}, Return Type: {method.ReturnType}",
                _ => $"Member: {member.Name}"
            });
    }
}