namespace Compiler.Syntax;

/// <summary>
/// A basic token that is used to represent a single unit of a program.
/// </summary>
/// <param name="Kind">The kind of token.</param>
/// <param name="Text">The text that was used to create the token.</param>
/// <param name="Value">The value of the token.</param>
/// <param name="Line">The line number of the token.</param>
/// <param name="Column">The column number of the token.</param>
public sealed record SyntaxToken
{
    public SyntaxKind Kind { get; }
    public int Line { get; }
    public int Column { get; }
    public string Text { get; }
    public object? Value { get; }

    public SyntaxToken(SyntaxKind kind, int line, int column, string? text, object? value)
    {
        Kind = kind;
        Text = text ?? SyntaxFacts.GetTokenText(kind) ?? throw new InvalidOperationException("No text for token.");
        Value = value;
        Line = line;
        Column = column;
    }

    public override string ToString() => $"{Kind}: {Text}";
}
