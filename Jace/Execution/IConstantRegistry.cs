using System.Collections.Generic;
using JetBrains.Annotations;

namespace Jace.Execution;

/// <summary>
/// Represents a registry for managing constants used in calculations.
/// </summary>
public interface IConstantRegistry : IEnumerable<ConstantInfo>
{
    /// <summary>Gets or sets a <see cref="ConstantInfo"/> object for a specified constant name.</summary>
    /// <param name="constantName">The name of the constant to retrieve.</param>
    /// <returns>The <see cref="ConstantInfo"/> associated with the specified constant name.</returns>
    [PublicAPI]
    ConstantInfo this[string constantName] { get; set; }

    /// <summary>
    /// Determines whether the specified name corresponds to a registered constant in the registry.
    /// </summary>
    /// <param name="constantName">The name of the constant to check.</param>
    /// <returns>if the provided name matches a registered constant; otherwise, false.</returns>
    bool Contains(string constantName);
    
    /// <summary>
    /// Attempts to retrieve constant information for the specified constant name.
    /// </summary>
    /// <param name="constantName">The name of the constant to retrieve.</param>
    /// <param name="constantInfo">When this method returns, contains the constant information associated with the specified name,
    /// if the name is found; otherwise, the default value for <see cref="ConstantInfo"/>.</param>
    /// <returns>True if the constant was found in the registry; otherwise, false.</returns>
    [PublicAPI]
    bool TryGetConstantInfo(string constantName, out ConstantInfo constantInfo);
    
    /// <summary>
    /// Registers a new constant in the registry or updates an existing one if it's overwritable.
    /// </summary>
    /// <param name="constantName">The name of the constant to register.</param>
    /// <param name="value">The numeric value of the constant.</param>
    /// <param name="isReadOnly">Indicates whether this constant can be overwritten or not. Defaults to false.</param>
    [PublicAPI]
    void RegisterConstant(string constantName, double value, bool isReadOnly = false);

    /// <summary>
    /// Registers multiple constants in the registry.
    /// </summary>
    /// <param name="constantInfos">An array of constants to be registered, including their names, values, and read-only status.</param>
    [PublicAPI]
    void RegisterConstants(params ConstantInfo[] constantInfos);
}