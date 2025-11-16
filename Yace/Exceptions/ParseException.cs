using System;

namespace Yace;

/// <summary>
/// The exception that is thrown when there is a syntax error in the formula provided
/// to the calculation engine.
/// </summary>
#pragma warning disable RCS1194
public sealed class ParseException(string message) : Exception(message);
#pragma warning restore RCS1194
