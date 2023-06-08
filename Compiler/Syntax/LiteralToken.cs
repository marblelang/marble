namespace Compiler.Syntax;

/// <summary>
/// A token that represents a literal value.
/// </summary>
/// <param name="Kind">The kind of token.</param>
/// <param name="Text">The text that was used to create the token.</param>
/// <param name="Value">The value of the literal.</param>
public sealed record LiteralToken : SyntaxToken
{
    public object Value { get; }

    public LiteralToken(SyntaxKind kind, string? text, object value) : base(kind, text)
    {
        Value = value;
    }

    public override string ToString() => $"{Kind}: {Text}";
}
