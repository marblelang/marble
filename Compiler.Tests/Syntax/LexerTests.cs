using Compiler.Diagnostics;
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

    private void AssertDiagnostics(params Diagnostic[] diagnostics)
    {
        var actual = Current.Diagnostics ?? Enumerable.Empty<DiagnosticInfo>();
        Assert.That(actual.Select(diagnostic => diagnostic.Diagnostic), Is.EquivalentTo(diagnostics));
    }

    private void AssertKind(SyntaxKind kind) => Assert.That(Current.Kind, Is.EqualTo(kind));

    private void AssertText(string text) => Assert.That(Current.Text, Is.EqualTo(text));

    private void AssertValue(object? value)
    {
        if (Current is LiteralToken token) Assert.That(token.Value, Is.EqualTo(value));
    }

    private void AssertLeadingTrivia(params (TriviaKind Kind, string Text)[] trivia)
    {
        var actual = Current.LeadingTrivia ?? Enumerable.Empty<TriviaToken>();
        Assert.That(actual.Select(token => (token.Kind, token.Text)), Is.EquivalentTo(trivia));
    }

    private void AssertTrailingTrivia(params (TriviaKind Kind, string Text)[] trivia)
    {
        var actual = Current.TrailingTrivia ?? Enumerable.Empty<TriviaToken>();
        Assert.That(actual.Select(token => (token.Kind, token.Text)), Is.EquivalentTo(trivia));
    }

    private void AssertNoTriviaOrDiagnostics()
    {
        AssertLeadingTrivia();
        AssertTrailingTrivia();
        AssertDiagnostics();
    }

    [Test]
    [Category("Trivia")]
    public void LineComment()
    {
        const string text = "// This is a comment";

        Lex(text);

        AssertNextToken(SyntaxKind.EndOfFile);
        AssertLeadingTrivia((TriviaKind.LineComment, text));
        AssertTrailingTrivia();
        AssertDiagnostics();
    }

    [Test]
    [Category("Trivia")]
    public void BlockCommentSingleLine()
    {
        const string text = "/* This is a comment */";

        Lex(text);

        AssertNextToken(SyntaxKind.EndOfFile);
        AssertLeadingTrivia((TriviaKind.MultilineComment, text));
        AssertTrailingTrivia();
        AssertDiagnostics();
    }

    [Test]
    [Category("Trivia")]
    public void BlockCommentMultiLine()
    {
        var text = """
        /* This is a comment
        * that spans multiple lines
        */
        """.Trim();

        Lex(text);

        AssertNextToken(SyntaxKind.EndOfFile);
        AssertLeadingTrivia((TriviaKind.MultilineComment, text));
        AssertTrailingTrivia();
        AssertDiagnostics();
    }

    [Test]
    [Category("Trivia")]
    public void UnclosedBlockComment()
    {
        var text = """
        /* This is a comment
        * that spans multiple lines
        """.Trim();

        Lex(text);

        AssertNextToken(SyntaxKind.EndOfFile);
        AssertLeadingTrivia((TriviaKind.MultilineComment, text));
        AssertTrailingTrivia();
        AssertDiagnostics(SyntaxDiagnostics.UnclosedComment);
    }

    [Test]
    [Category("String")]
    public void LineString()
    {
        const string text = @"""This is a string""";

        Lex(text);

        AssertNextToken(SyntaxKind.LineStringStart, "\"");
        AssertNoTriviaOrDiagnostics();

        AssertNextToken(SyntaxKind.StringLiteral, "This is a string");
        AssertNoTriviaOrDiagnostics();

        AssertNextToken(SyntaxKind.LineStringEnd, "\"");
        AssertNoTriviaOrDiagnostics();

        AssertNextToken(SyntaxKind.EndOfFile);
        AssertNoTriviaOrDiagnostics();
    }

    [Test]
    [Category("String")]
    public void UnclosedLineString()
    {
        const string text = @"""This is a string";

        Lex(text);

        AssertNextToken(SyntaxKind.LineStringStart, "\"");
        AssertNoTriviaOrDiagnostics();

        AssertNextToken(SyntaxKind.StringLiteral, "This is a string");
        AssertNoTriviaOrDiagnostics();

        AssertNextToken(SyntaxKind.EndOfFile);
        AssertDiagnostics(SyntaxDiagnostics.UnclosedString);
    }

    [Test]
    [Category("String")]
    public void MultilineString()
    {
        var text = @"
""""""This is a string
that spans multiple lines""""""
        ".Trim();

        Lex(text);

        AssertNextToken(SyntaxKind.MultilineStringStart, "\"\"\"");
        AssertNoTriviaOrDiagnostics();

        AssertNextToken(SyntaxKind.StringLiteral, "This is a string\nthat spans multiple lines");
        AssertNoTriviaOrDiagnostics();

        AssertNextToken(SyntaxKind.MultilineStringEnd, "\"\"\"");
        AssertNoTriviaOrDiagnostics();

        AssertNextToken(SyntaxKind.EndOfFile);
        AssertNoTriviaOrDiagnostics();
    }

    [Test]
    [Category("String")]
    public void UnclosedMultilineString()
    {
        var text = @"
""""""This is a string
that spans multiple lines""""
        ".Trim();

        Lex(text);

        AssertNextToken(SyntaxKind.MultilineStringStart, "\"\"\"");
        AssertNoTriviaOrDiagnostics();

        AssertNextToken(SyntaxKind.StringLiteral, "This is a string\nthat spans multiple lines\"\"");
        AssertNoTriviaOrDiagnostics();

        AssertNextToken(SyntaxKind.EndOfFile);
        AssertDiagnostics(SyntaxDiagnostics.UnclosedString);
    }

    [Test]
    [TestCase("do", SyntaxKind.DoKeyword)]
    [TestCase("else", SyntaxKind.ElseKeyword)]
    [TestCase("false", SyntaxKind.FalseKeyword)]
    [TestCase("for", SyntaxKind.ForKeyword)]
    [TestCase("fun", SyntaxKind.FunKeyword)]
    [TestCase("if", SyntaxKind.IfKeyword)]
    [TestCase("return", SyntaxKind.ReturnKeyword)]
    [TestCase("true", SyntaxKind.TrueKeyword)]
    [TestCase("use", SyntaxKind.UseKeyword)]
    [TestCase("val", SyntaxKind.ValKeyword)]
    [TestCase("var", SyntaxKind.VarKeyword)]
    [TestCase("while", SyntaxKind.WhileKeyword)]
    [Category("Syntax")]
    public void Keyword(string text, SyntaxKind kind)
    {
        Lex(text);

        AssertNextToken(kind, text);
        AssertNoTriviaOrDiagnostics();

        AssertNextToken(SyntaxKind.EndOfFile);
        AssertNoTriviaOrDiagnostics();
    }

    [Test]
    [TestCase("{", SyntaxKind.OpenBrace)]
    [TestCase("}", SyntaxKind.CloseBrace)]
    [TestCase("(", SyntaxKind.OpenParenthesis)]
    [TestCase(")", SyntaxKind.CloseParenthesis)]
    [TestCase("[", SyntaxKind.OpenBracket)]
    [TestCase("]", SyntaxKind.CloseBracket)]
    [TestCase(",", SyntaxKind.Comma)]
    [TestCase(":", SyntaxKind.Colon)]
    [TestCase(";", SyntaxKind.Semicolon)]
    [TestCase(".", SyntaxKind.Period)]
    [Category("Syntax")]
    public void Punctuation(string text, SyntaxKind kind)
    {
        Lex(text);

        AssertNextToken(kind, text);
        AssertNoTriviaOrDiagnostics();

        AssertNextToken(SyntaxKind.EndOfFile);
        AssertNoTriviaOrDiagnostics();
    }

    [Test]
    [TestCase("+", SyntaxKind.Plus)]
    [TestCase("-", SyntaxKind.Minus)]
    [TestCase("*", SyntaxKind.Asterisk)]
    [TestCase("/", SyntaxKind.Slash)]
    [TestCase("%", SyntaxKind.Percent)]
    [TestCase("^", SyntaxKind.Caret)]
    [TestCase("&", SyntaxKind.Ampersand)]
    [TestCase("!", SyntaxKind.ExclamationMark)]
    [TestCase("?", SyntaxKind.QuestionMark)]
    [TestCase("=", SyntaxKind.Equals)]
    [TestCase("<", SyntaxKind.LessThan)]
    [TestCase(">", SyntaxKind.GreaterThan)]
    [TestCase("|", SyntaxKind.Pipe)]
    [TestCase("==", SyntaxKind.DoubleEquals)]
    [TestCase("!=", SyntaxKind.NotEquals)]
    [TestCase("<=", SyntaxKind.LessThanOrEqual)]
    [TestCase(">=", SyntaxKind.GreaterThanOrEqual)]
    [TestCase("&&", SyntaxKind.DoubleAmpersand)]
    [TestCase("||", SyntaxKind.DoublePipe)]
    [TestCase("+=", SyntaxKind.PlusEquals)]
    [TestCase("-=", SyntaxKind.MinusEquals)]
    [TestCase("*=", SyntaxKind.AsteriskEquals)]
    [TestCase("/=", SyntaxKind.SlashEquals)]
    [TestCase("%=", SyntaxKind.PercentEquals)]
    [TestCase("^=", SyntaxKind.CaretEquals)]
    [TestCase("++", SyntaxKind.DoublePlus)]
    [TestCase("--", SyntaxKind.DoubleMinus)]
    [TestCase("!!", SyntaxKind.DoubleExclamationMark)]
    [TestCase("??", SyntaxKind.DoubleQuestionMark)]
    [TestCase("?.", SyntaxKind.QuestionMarkPeriod)]
    [Category("Syntax")]
    public void Operator(string text, SyntaxKind kind)
    {
        Lex(text);

        AssertNextToken(kind, text);
        AssertNoTriviaOrDiagnostics();

        AssertNextToken(SyntaxKind.EndOfFile);
        AssertNoTriviaOrDiagnostics();
    }
}
