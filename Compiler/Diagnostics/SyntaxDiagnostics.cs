namespace Compiler.Diagnostics;

internal static class SyntaxDiagnostics
{
    private const DiagnosticCategory Category = DiagnosticCategory.Syntax;

    private enum Index
    {
        InvalidToken = 0,
        UnclosedString = 1,
        UnclosedComment = 2,
        UnclosedChar = 3,
        InvalidChar = 4,
        InvalidUnicodeCodepoint = 5,
        InvalidEscapeCharacter = 6
    }

    private static string Code (Index index) => Diagnostic.CreateDiagnosticCode(Category, (int) index);

    public static readonly Diagnostic InvalidToken = new(
        Code(Index.InvalidToken),
        "invalid token",
        "invalid token '{0}'",
        Category,
        DiagnosticSeverity.Error);

    public static readonly Diagnostic UnclosedString = new(
        Code(Index.UnclosedString),
        "unclosed string",
        "unclosed string",
        Category,
        DiagnosticSeverity.Error);

    public static readonly Diagnostic UnclosedComment = new(
        Code(Index.UnclosedComment),
        "unclosed comment",
        "unclosed comment",
        Category,
        DiagnosticSeverity.Error);

    public static readonly Diagnostic UnclosedChar = new(
        Code(Index.UnclosedChar),
        "unclosed character literal",
        "unclosed character literal",
        Category,
        DiagnosticSeverity.Error);

    public static readonly Diagnostic InvalidChar = new(
        Code(Index.InvalidChar),
        "invalid character literal",
        "invalid character literal",
        Category,
        DiagnosticSeverity.Error);

    public static readonly Diagnostic InvalidUnicodeCodepoint = new(
        Code(Index.InvalidUnicodeCodepoint),
        "invalid unicode codepoint",
        "invalid unicode codepoint",
        Category,
        DiagnosticSeverity.Error);

    public static readonly Diagnostic InvalidEscapeCharacter = new(
        Code(Index.InvalidEscapeCharacter),
        "invalid escape character",
        "invalid escape character '{0}'",
        Category,
        DiagnosticSeverity.Error);
}
