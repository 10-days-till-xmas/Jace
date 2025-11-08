using System;
using System.Linq;
using System.Reflection;

namespace Yace.Util;

public static class TypeExtensions
{
    /// <summary>
    /// Gets the constructor for a given type matching with the parameter types provided.
    /// </summary>
    /// <param name="type">The type for witch a matching constructor must be found.</param>
    /// <param name="parameters">The parameter types of the constructor.</param>
    /// <returns>The matching constructor.</returns>
    public static ConstructorInfo GetConstructor(this Type type, Type[] parameters)
    {
        var constructors =
            type.GetTypeInfo().DeclaredConstructors.Where(c => c.GetParameters().Length == parameters.Length);

        foreach (var constructor in constructors)
        {
            var parametersMatch = true;

            var constructorParameters = constructor.GetParameters();
            for (var i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] != constructorParameters[i].ParameterType)
                {
                    parametersMatch = false;
                    break;
                }
            }

            if (parametersMatch)
                return constructor;
        }

        throw new Exception("No constructor was found matching with the provided parameters.");
    }
}