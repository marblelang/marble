using System.Text;

namespace Compiler.Syntax;

internal sealed class Lexer
{
    private int _column = 1;
    private int _line = 1;

    private SourceReader Reader { get; }

    public List<Diagnostic> Diagnostics { get; } = new();

    public Lexer(SourceReader reader)
    {
        Reader = reader;
    }

    /// <summary>
    /// Reads the next <see cref="SyntaxToken"/> from the <see cref="Lexer.Reader"/>.
    /// </summary>
    /// <returns>The <see cref="SyntaxToken"/>.</returns>
    public SyntaxToken Lex()
    {
        var ch = Reader.Peek();

        if (ch == '/' && Reader.Peek(1) == '/')
        {
            var offset = 2;
            while (Reader.Peek(offset) != '\n' && Reader.Peek(offset) != '\r') offset++;
            return TakeWithText(SyntaxKind.LineComment, offset);
        }

        switch (ch)
        {
            case '\0': return new SyntaxToken(SyntaxKind.EndOfFile, _line, _column, null, null);
            case '\n':
                _column = 1;
                _line++;
                return TakeBasic(SyntaxKind.NewLine, 1);
            case '\r':
                _column = 1;
                _line++;
                return TakeBasic(SyntaxKind.NewLine, Reader.Peek(1) == '\n' ? 2 : 1);
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

        if (char.IsDigit(ch))
        {
            var offset = 1;
            while (char.IsDigit(Reader.Peek(offset))) offset++;
            var view = Reader.Read(offset);
            _column += offset;
            var value = int.Parse(view.Span);
            return new SyntaxToken(SyntaxKind.NumberLiteral, _line, _column, view.ToString(), value);
        }

        if (IsIdentifierStart(ch))
        {
            var offset = 1;
            while (IsIdentifierPart(Reader.Peek(offset))) offset++;
            var view = Reader.Read(offset);
            _column += offset;
            var text = view.ToString();
            var kind = SyntaxFacts.GetKeywordKind(text);
            return new SyntaxToken(kind, _line, _column, text, text);
        }

        if (char.IsWhiteSpace(ch))
        {
            var offset = 1;
            while (char.IsWhiteSpace(Reader.Peek(offset))) offset++;
            _column += offset;
            return TakeWithText(SyntaxKind.Whitespace, offset);
        }

        // Character literals
        if (ch == '\'')
        {
            var offset = 1;
            var ch2 = Reader.Peek(offset);
            var result = '\0';

            if (ch2 == '\\')
            {
                var ch3 = Reader.Peek(offset + 1);
                // TODO: Unicode
                switch (ch3)
                {
                    case 'n': result = '\n'; break;
                    case 'r': result = '\r'; break;
                    case 't': result = '\t'; break;
                    case '\'': result = '\''; break;
                    case '\"': result = '\"'; break;
                    case '\\': result = '\\'; break;
                    default:
                        AddError(_column + offset, _line, $"Invalid escape sequence '\\{ch3}'");
                        break;
                }
                offset += 2;
            }
            else if (!char.IsControl(ch2))
            {
                result = ch2;
                offset++;
            }
            else
            {
                AddError(_column + offset, _line, $"Invalid character literal '{ch2}'");
                offset++;
                result = ' ';
            }

            if (Reader.Peek(offset) != '\'')
            {
                AddError(_column + offset, _line, "Expected closing '");
            }
            else
            {
                offset++;
            }

            _column += offset;
            return new SyntaxToken(SyntaxKind.CharacterLiteral, _line, _column, Reader.Read(offset).ToString(), result);
        }

        // String literals
        if (ch == '\"')
        {
            var offset = 1;
            var builder = new StringBuilder();
            while (true)
            {
                var ch2 = Reader.Peek(offset);

                if (ch2 == '\"')
                {
                    offset++;
                    break;
                }

                if (ch2 == '\0')
                {
                    AddError(_column + offset, _line, "Expected closing \"");
                    break;
                }

                if (!char.IsControl(ch2))
                {
                    builder.Append(ch2);
                    offset++;
                }
                else
                {
                    AddError(_column + offset, _line, $"Invalid character literal '{ch2}'");
                    offset++;
                    builder.Append(' ');
                }
            }

            _column += offset;
            return new SyntaxToken(SyntaxKind.StringLiteral, _line, _column, Reader.Read(offset).ToString(), builder.ToString());
        }

        return TakeWithText(SyntaxKind.Unknown, 1);
    }

    private void AddError(int column, int line, string message)
    {
        Diagnostics.Add(new Diagnostic(DiagnosticSeverity.Error, message, line, column));
    }

    private SyntaxToken TakeBasic(SyntaxKind kind, int length)
    {
        var result = new SyntaxToken(kind, _line, _column, null, null);
        Reader.Read(length);
        _column += length;
        return result;
    }

    private SyntaxToken TakeWithText(SyntaxKind kind, int length)
    {
        var result = new SyntaxToken(kind, _line, _column, Reader.Read(length).ToString(), null);
        _column += length;
        return result;
    }

    private static bool IsIdentifierStart(char ch) => char.IsLetter(ch);
    private static bool IsIdentifierPart(char ch) => char.IsLetterOrDigit(ch) || ch == '_';
}
