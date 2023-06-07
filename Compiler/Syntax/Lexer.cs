using Compiler.Diagnostics;

namespace Compiler.Syntax;

internal sealed class Lexer
{
    private enum LexerState
    {
        Normal,
        Interpolation,
        LineString,
        MultilineString
    }

    private Stack<LexerState> State { get; } = new();
    private SourceReader Reader { get; }

    public List<DiagnosticInfo> Diagnostics { get; } = new();

    public Lexer(SourceReader reader)
    {
        Reader = reader;
        State.Push(LexerState.Normal);
    }

    /// <summary>
    /// Reads the next <see cref="SyntaxToken"/> from the <see cref="Lexer.Reader"/>.
    /// </summary>
    /// <returns>The <see cref="SyntaxToken"/>.</returns>
    public SyntaxToken Lex()
    {
        SyntaxToken token;

        switch (State.Peek())
        {
            case LexerState.Normal:
            case LexerState.Interpolation:
                var leadingTrivia = ParseLeadingTrivia();
                token = LexNormal();
                var trailingTrivia = ParseTrailingTrivia();

                token.LeadingTrivia = leadingTrivia;
                token.TrailingTrivia = trailingTrivia;
                break;
            case LexerState.LineString:
                token = LexLineString();
                break;
            case LexerState.MultilineString:
                token = LexMultilineString();
                break;
            default:
                throw new InvalidOperationException("Invalid lexer state");
        }

        return token;
    }

    private SyntaxToken LexNormal()
    {
        var ch = Reader.Peek();

        switch (ch)
        {
            case '\0': return new SyntaxToken(SyntaxKind.EndOfFile, null, null);
            case '(': return TakeBasic(SyntaxKind.OpenParenthesis, 1);
            case ')': return TakeBasic(SyntaxKind.CloseParenthesis, 1);
            case '{': return TakeBasic(SyntaxKind.OpenBrace, 1);
            case '}': return TakeBasic(SyntaxKind.CloseBrace, 1);
            case '[': return TakeBasic(SyntaxKind.OpenBracket, 1);
            case ']': return TakeBasic(SyntaxKind.CloseBracket, 1);
            case ',': return TakeBasic(SyntaxKind.Comma, 1);
            case ':': return TakeBasic(SyntaxKind.Colon, 1);
            case ';': return TakeBasic(SyntaxKind.Semicolon, 1);
            case '.': return TakeBasic(SyntaxKind.Period, 1);
            case '+': return Reader.Peek(1) == '=' ? TakeBasic(SyntaxKind.PlusEquals, 2) : TakeBasic(SyntaxKind.Plus, 1);
            case '-': return Reader.Peek(1) == '=' ? TakeBasic(SyntaxKind.MinusEquals, 2) : TakeBasic(SyntaxKind.Minus, 1);
            case '*': return Reader.Peek(1) == '=' ? TakeBasic(SyntaxKind.AsteriskEquals, 2) : TakeBasic(SyntaxKind.Asterisk, 1);
            case '/': return Reader.Peek(1) == '=' ? TakeBasic(SyntaxKind.SlashEquals, 2) : TakeBasic(SyntaxKind.Slash, 1);
            case '%': return Reader.Peek(1) == '=' ? TakeBasic(SyntaxKind.PercentEquals, 2) : TakeBasic(SyntaxKind.Percent, 1);
            case '^': return Reader.Peek(1) == '=' ? TakeBasic(SyntaxKind.CaretEquals, 2) : TakeBasic(SyntaxKind.Caret, 1);
            case '&': return Reader.Peek(1) == '&' ? TakeBasic(SyntaxKind.DoubleAmpersand, 2) : TakeBasic(SyntaxKind.Ampersand, 1);
            case '|': return Reader.Peek(1) == '|' ? TakeBasic(SyntaxKind.DoublePipe, 2) : TakeBasic(SyntaxKind.Pipe, 1);
            case '=': return Reader.Peek(1) == '=' ? TakeBasic(SyntaxKind.DoubleEquals, 2) : TakeBasic(SyntaxKind.Equals, 1);
            case '!': return Reader.Peek(1) == '=' ? TakeBasic(SyntaxKind.NotEquals, 2) : TakeBasic(SyntaxKind.ExclamationMark, 1);
            case '<': return Reader.Peek(1) == '=' ? TakeBasic(SyntaxKind.LessThanOrEqual, 2) : TakeBasic(SyntaxKind.LessThan, 1);
            case '>': return Reader.Peek(1) == '=' ? TakeBasic(SyntaxKind.GreaterThanOrEqual, 2) : TakeBasic(SyntaxKind.GreaterThan, 1);
        }

        // Numeric literals
        // TODO: Handle scientific notation
        if (char.IsDigit(ch))
        {
            var offset = 1;
            while (char.IsDigit(Reader.Peek(offset))) offset++;

            ch = Reader.Peek(offset);

            if (ch == '.')
            {
                // Floating point
                offset++;
                while (char.IsDigit(Reader.Peek(offset))) offset++;

                ch = Reader.Peek(offset);

                if (ch == 'f')
                {
                    var view = Reader.Read(offset);
                    var value = float.Parse(view.Span);
                    Reader.Read(1); // Skip the 'f'
                    return new SyntaxToken(SyntaxKind.FloatLiteral, view.ToString(), value);
                }
                else
                {
                    var view = Reader.Read(offset);
                    var value = double.Parse(view.Span);
                    return new SyntaxToken(SyntaxKind.DoubleLiteral, view.ToString(), value);
                }
            }

            // We need this in a scope so we can reuse the variable names
            {
                // Integer
                var view = Reader.Read(offset);
                var value = int.Parse(view.Span);
                return new SyntaxToken(SyntaxKind.IntegerLiteral, view.ToString(), value);
            }
        }

        if (IsIdentifierStart(ch))
        {
            var offset = 1;
            while (IsIdentifierPart(Reader.Peek(offset))) offset++;
            var view = Reader.Read(offset);
            var text = view.ToString();
            var kind = SyntaxFacts.GetKeywordKind(text);
            return new SyntaxToken(kind, text, text);
        }

        // Character literals
        if (ch == '\'')
        {
            var offset = 1;
            var ch2 = Reader.Peek(offset);
            char result;

            if (ch2 == '\\')
            {
                offset++;
                result = ParseEscapeSequence(ref offset);
            }
            else if (!char.IsControl(ch2))
            {
                offset++;
                result = ch2;
            }
            else
            {
                AddError(SyntaxDiagnostics.InvalidChar, offset, 1);
                offset++;
                result = ' ';
            }

            if (Reader.Peek(offset) != '\'')
            {
                AddError(SyntaxDiagnostics.UnclosedChar, offset, 1);
            }
            else
            {
                offset++;
            }

            return new SyntaxToken(SyntaxKind.CharacterLiteral, Reader.Read(offset).ToString(), result);
        }

        // String literals
        if (ch == '\"')
        {
            if (Reader.Peek(1) == '\"' && Reader.Peek(2) == '\"')
            {
                State.Push(LexerState.MultilineString);
                return TakeBasic(SyntaxKind.MultilineStringStart, 3);
            }

            State.Push(LexerState.LineString);
            return TakeBasic(SyntaxKind.LineStringStart, 1);
        }

        return TakeWithText(SyntaxKind.Unknown, 1);
    }

    private SyntaxToken LexLineString()
    {
        var offset = 0;
        var ch = Reader.Peek(offset);

        switch (ch)
        {
            case '\"':
                State.Pop();
                return TakeBasic(SyntaxKind.LineStringEnd, 1);
            case '\0' or '\r' or '\n':
                AddError(SyntaxDiagnostics.UnclosedString, offset, 1);
                State.Pop();
                switch (ch)
                {
                    case '\0': return new SyntaxToken(SyntaxKind.EndOfFile, null, null);
                    case '\r' or '\n': return ParseNewline(ref offset);
                }
                break;
        }

        while (ch != '\0' && ch != '\r' && ch != '\n' && ch != '\"')
        {
            if (ch == '\\')
            {
                offset++;
                ch = Reader.Peek(offset);
                if (ch is '\0' or '\r' or '\n')
                {
                    AddError(SyntaxDiagnostics.UnclosedString, offset, 1);
                    break;
                }
            }

            offset++;
            ch = Reader.Peek(offset);
        }

        var text = Reader.Read(offset).ToString();

        return new SyntaxToken(SyntaxKind.StringLiteral, text, text);
    }

    private SyntaxToken LexMultilineString()
    {
        var offset = 0;
        var ch = Reader.Peek(offset);
        var ch2 = Reader.Peek(1);
        var ch3 = Reader.Peek(2);

        switch (ch)
        {
            case '\"':
                if (ch2 is '\"' && ch3 is '\"')
                {
                    State.Pop();
                    return TakeBasic(SyntaxKind.MultilineStringEnd, 3);
                }

                break;
            case '\0':
                AddError(SyntaxDiagnostics.UnclosedString, offset, 1);
                State.Pop();
                return new SyntaxToken(SyntaxKind.EndOfFile, null, null);
        }

        while (ch != '\0' && !(ch is '\"' && ch2 is '\"' && ch3 is '\"'))
        {
            if (ch == '\\')
            {
                offset++;
                ch = Reader.Peek(offset);
                if (ch is '\0')
                {
                    AddError(SyntaxDiagnostics.UnclosedString, offset, 1);
                    break;
                }
            }

            offset++;
            ch = Reader.Peek(offset);
            ch2 = Reader.Peek(offset + 1);
            ch3 = Reader.Peek(offset + 2);
        }

        var text = Reader.Read(offset).ToString();

        return new SyntaxToken(SyntaxKind.StringLiteral, text, text);
    }

    private void AddError(Diagnostic diagnostic, int offset, int width, params object[] args)
    {
        Diagnostics.Add(new DiagnosticInfo(diagnostic, offset, width, args));
    }

    private char ParseEscapeSequence(ref int offset)
    {
        var ch = Reader.Peek(offset);

        // Check for Unicode first in the form \uXXXX
        if (ch == 'u')
        {
            offset++;
            var value = 0;
            for (var i = 0; i < 4; i++)
            {
                ch = Reader.Peek(offset + i);
                switch (ch)
                {
                    case >= '0' and <= '9':
                        value = (value << 4) + (ch - '0');
                        break;
                    case >= 'a' and <= 'f':
                        value = (value << 4) + (ch - 'a' + 10);
                        break;
                    case >= 'A' and <= 'F':
                        value = (value << 4) + (ch - 'A' + 10);
                        break;
                    default:
                        AddError(SyntaxDiagnostics.InvalidUnicodeCodepoint, offset, 4);
                        return ' ';
                }
            }

            offset += 4;
            return (char)value;
        }

        // Check for other escape sequences
        switch (ch)
        {
            case '0': offset++; return '\0';
            case 'a': offset++; return '\a';
            case 'b': offset++; return '\b';
            case 'f': offset++; return '\f';
            case 'n': offset++; return '\n';
            case 'r': offset++; return '\r';
            case 't': offset++; return '\t';
            case 'v': offset++; return '\v';
            case '\\': offset++; return '\\';
            case '\'': offset++; return '\'';
            case '\"': offset++; return '\"';
            default:
                AddError(SyntaxDiagnostics.InvalidEscapeCharacter, offset, 1);
                offset++;
                return ' ';
        }
    }

    private SyntaxToken ParseNewline(ref int offset)
    {
        var ch = Reader.Peek(offset);

        switch (ch)
        {
            case '\r' when Reader.Peek(1) == '\n':
                offset += 2;
                return TakeBasic(SyntaxKind.Newline, 2);
            case '\r' or '\n':
                offset++;
                return TakeBasic(SyntaxKind.Newline, 1);
            default:
                return TakeWithText(SyntaxKind.Unknown, 1);
        }
    }

    private List<SyntaxToken> ParseLeadingTrivia()
    {
        var result = new List<SyntaxToken>();
        while (true)
        {
            var trivia = ParseTrivia();
            if (trivia == null) break;
            result.Add(trivia);
        }
        return result;
    }

    private List<SyntaxToken> ParseTrailingTrivia()
    {
        var result = new List<SyntaxToken>();
        while (true)
        {
            var trivia = ParseTrivia();
            if (trivia == null || trivia.Kind == SyntaxKind.Newline) break;
            result.Add(trivia);
        }
        return result;
    }

    private SyntaxToken? ParseTrivia()
    {
        var ch = Reader.Peek();

        // Newline
        if (ch is '\r' or '\n')
        {
            var offset = 1;
            if (ch == '\r' && Reader.Peek(offset) == '\n') offset++;
            return TakeBasic(SyntaxKind.Newline, offset);
        }

        // Horizontal whitespace, including tabs and spaces
        if (char.IsWhiteSpace(ch))
        {
            var offset = 1;
            while (char.IsWhiteSpace(Reader.Peek(offset))) offset++;
            return TakeWithText(SyntaxKind.Whitespace, offset);
        }

        // Single-line comment
        if (ch == '/' && Reader.Peek(1) == '/')
        {
            var offset = 2;
            while (Reader.Peek(offset) != '\r' && Reader.Peek(offset) != '\n' && Reader.Peek(offset) != '\0') offset++;
            return TakeWithText(SyntaxKind.LineComment, offset);
        }

        // Multi-line comment
        if (ch == '/' && Reader.Peek(1) == '*')
        {
            var offset = 2;
            while (true)
            {
                if (Reader.Peek(offset) == '\0')
                {
                    AddError(SyntaxDiagnostics.UnclosedComment, offset, 1);
                    break;
                }

                if (Reader.Peek(offset) == '*' && Reader.Peek(offset + 1) == '/')
                {
                    offset += 2;
                    break;
                }

                offset++;
            }
            return TakeWithText(SyntaxKind.MultilineComment, offset);
        }

        return null;
    }

    private SyntaxToken TakeBasic(SyntaxKind kind, int length)
    {
        var result = new SyntaxToken(kind, null, null);
        Reader.Read(length);
        return result;
    }

    private SyntaxToken TakeWithText(SyntaxKind kind, int length)
    {
        var result = new SyntaxToken(kind, Reader.Read(length).ToString(), null);
        return result;
    }

    private static bool IsIdentifierStart(char ch) => char.IsLetter(ch);
    private static bool IsIdentifierPart(char ch) => char.IsLetterOrDigit(ch) || ch == '_';
}
