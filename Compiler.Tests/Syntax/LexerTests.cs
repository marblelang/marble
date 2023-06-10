using Compiler.Syntax;

namespace Compiler.Tests.Syntax;

public class LexerTests
{
    private IEnumerator<SyntaxToken> _tokenEnumerator = Enumerable.Empty<SyntaxToken>().GetEnumerator();
    private SyntaxToken Current => _tokenEnumerator.Current;

    private void Lex(string text)
    {
        var srcReader = new SourceReader(text);
        var lexer = new Lexer(srcReader);

        _tokenEnumerator = LexAll(lexer).GetEnumerator();
    }

    private static IEnumerable<SyntaxToken> LexAll(Lexer lexer)
    {
        while (true)
        {
            var token = lexer.Lex();
            yield return token;
            if (token.Kind == SyntaxKind.EndOfFile) break;
        }
    }

    private void AssertNextToken() => Assert.That(_tokenEnumerator.MoveNext(), Is.True);

    private void AssertNextToken(SyntaxKind kind, string text = "", object? value = null)
    {
        Assert.Multiple(() =>
        {
            AssertNextToken();
            AssertKind(kind);
            AssertText(text);
            if (value is not null) AssertValue(value);
        });
    }

    private void AssertKind(SyntaxKind kind) => Assert.That(Current.Kind, Is.EqualTo(kind));

    private void AssertText(string text) => Assert.That(Current.Text, Is.EqualTo(text));

    private void AssertValue(object? value)
    {
        if (Current is LiteralToken token) Assert.That(token.Value, Is.EqualTo(value));
    }

    [Test]
    public void LineComment()
    {
        const string text = "// This is a comment";

        Lex(text);

        AssertNextToken(SyntaxKind.EndOfFile);
    }
}
