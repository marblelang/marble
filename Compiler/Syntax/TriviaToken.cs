namespace Compiler.Syntax;

/// <summary>
/// A token that represents a piece of trivia.
/// </summary>
/// <param name="Kind">The kind of token.</param>
/// <param name="Text">The text that was used to create the token.</param>
public sealed record TriviaToken(TriviaKind Kind, string Text) : IToken;
