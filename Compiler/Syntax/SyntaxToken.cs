namespace Compiler.Syntax;

/// <summary>
/// A basic token with trivia that is used to represent a single unit of a program.
/// </summary>
/// <param name="Kind">The kind of token.</param>
/// <param name="Text">The text that was used to create the token.</param>
/// <param name="LeadingTrivia">The trivia that comes before the token.</param>
/// <param name="TrailingTrivia">The trivia that comes after the token.</param>
public record SyntaxToken : IToken
{
    public SyntaxKind Kind { get; }
    public string Text { get; }
    public List<TriviaToken>? LeadingTrivia { get; set; }
    public List<TriviaToken>? TrailingTrivia { get; set; }

    public SyntaxToken(SyntaxKind kind, string? text)
    {
        Kind = kind;
        Text = text ?? SyntaxFacts.GetTokenText(kind) ?? throw new InvalidOperationException("No text for token.");
    }

    public override string ToString() => $"{Kind}: {Text}";
}
