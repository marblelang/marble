namespace Compiler.Syntax;

public enum TriviaKind
{
    /// <summary>
    /// A new line sequence.
    /// </summary>
    Newline,

    /// <summary>
    /// Horizontal whitespace.
    /// </summary>
    Whitespace,

    /// <summary>
    /// A single line comment.
    /// </summary>
    LineComment,

    /// <summary>
    /// A multi-line comment.
    /// </summary>
    MultilineComment
}
