namespace Compiler.Syntax;

/// <summary>
/// Utilities for working with syntax.
/// </summary>
public static class SyntaxFacts
{
    /// <summary>
    /// Gets the text of a <see cref="SyntaxKind"/>, if possible.
    /// </summary>
    /// <param name="kind">The <see cref="SyntaxKind"/> to get the text for.</param>
    /// <returns>The text of <paramref name="kind"/> or null if there is no representation.</returns>
    public static string? GetTokenText(SyntaxKind kind) => kind switch
    {
        SyntaxKind.EndOfFile => string.Empty,
        SyntaxKind.NewLine => "\\n", // This is a special case.
        SyntaxKind.OpenParenthesis => "(",
        SyntaxKind.CloseParenthesis => ")",
        SyntaxKind.OpenBrace => "{",
        SyntaxKind.CloseBrace => "}",
        SyntaxKind.OpenBracket => "[",
        SyntaxKind.CloseBracket => "]",
        SyntaxKind.Comma => ",",
        SyntaxKind.Colon => ":",
        SyntaxKind.Semicolon => ";",
        SyntaxKind.Plus => "+",
        SyntaxKind.Minus => "-",
        SyntaxKind.Asterisk => "*",
        SyntaxKind.Slash => "/",
        SyntaxKind.Percent => "%",
        SyntaxKind.Caret => "^",
        SyntaxKind.Ampersand => "&",
        SyntaxKind.ExclamationMark => "!",
        SyntaxKind.QuestionMark => "?",
        SyntaxKind.Equals => "=",
        SyntaxKind.Period => ".",
        SyntaxKind.LessThan => "<",
        SyntaxKind.GreaterThan => ">",
        SyntaxKind.SingleQuote => "'",
        SyntaxKind.DoubleQuote => "\"",
        SyntaxKind.Backslash => "\\",
        SyntaxKind.Pipe => "|",
        SyntaxKind.DoubleEquals => "==",
        SyntaxKind.NotEquals => "!=",
        SyntaxKind.LessThanOrEqual => "<=",
        SyntaxKind.GreaterThanOrEqual => ">=",
        SyntaxKind.DoubleAmpersand => "&&",
        SyntaxKind.DoublePipe => "||",
        SyntaxKind.PlusEquals => "+=",
        SyntaxKind.MinusEquals => "-=",
        SyntaxKind.AsteriskEquals => "*=",
        SyntaxKind.SlashEquals => "/=",
        SyntaxKind.PercentEquals => "%=",
        SyntaxKind.CaretEquals => "^=",
        SyntaxKind.DoublePlus => "++",
        SyntaxKind.DoubleMinus => "--",
        SyntaxKind.AndKeyword => "and",
        SyntaxKind.DoKeyword => "do",
        SyntaxKind.ElseKeyword => "else",
        SyntaxKind.FalseKeyword => "false",
        SyntaxKind.ForKeyword => "for",
        SyntaxKind.FunKeyword => "fun",
        SyntaxKind.IfKeyword => "if",
        SyntaxKind.OrKeyword => "or",
        SyntaxKind.ReturnKeyword => "return",
        SyntaxKind.TrueKeyword => "true",
        SyntaxKind.ValKeyword => "val",
        SyntaxKind.VarKeyword => "var",
        SyntaxKind.WhileKeyword => "while",
        _ => null
    };

    /// <summary>
    /// Gets the <see cref="SyntaxKind"/> of a keyword. If the text is not a keyword, <see cref="SyntaxKind.Identifier"/> is returned.
    /// </summary>
    /// <param name="text">The text to get the <see cref="SyntaxKind"/> for.</param>
    /// <returns>The <see cref="SyntaxKind"/> of <paramref name="text"/>.</returns>
    public static SyntaxKind GetKeywordKind(string text) => text switch
    {
        "and" => SyntaxKind.AndKeyword,
        "do" => SyntaxKind.DoKeyword,
        "else" => SyntaxKind.ElseKeyword,
        "false" => SyntaxKind.FalseKeyword,
        "for" => SyntaxKind.ForKeyword,
        "fun" => SyntaxKind.FunKeyword,
        "if" => SyntaxKind.IfKeyword,
        "or" => SyntaxKind.OrKeyword,
        "return" => SyntaxKind.ReturnKeyword,
        "true" => SyntaxKind.TrueKeyword,
        "use" => SyntaxKind.UseKeyword,
        "val" => SyntaxKind.ValKeyword,
        "var" => SyntaxKind.VarKeyword,
        "while" => SyntaxKind.WhileKeyword,
        _ => SyntaxKind.Identifier
    };
}
