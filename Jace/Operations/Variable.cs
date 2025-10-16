namespace Jace.Operations;

/// <summary>
/// Represents a variable in a mathematical formula.
/// </summary>
public class Variable : Operation
{
    public Variable(string name)
        : base(DataType.FloatingPoint, true, false)
    {
        Name = name;
    }

    public string Name { get; private set; }

    public override bool Equals(object obj)
    {
        var other = obj as Variable;
        if (other != null)
        {
            return Name.Equals(other.Name);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}