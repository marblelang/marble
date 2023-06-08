namespace Compiler.Syntax;

/// <summary>
/// A basic token that is used to represent a single unit of a program.
/// </summary>
/// <param name="Text">The text that was used to create the token.</param>
public interface IToken
{
    public string Text { get; }
}
