using System;

namespace Yace;

/// <summary>
/// An exception thrown when a formula must be executed with a variable that is not defined.
/// </summary>
#pragma warning disable RCS1194
public sealed class VariableNotDefinedException(string message) : Exception(message);
#pragma warning restore RCS1194