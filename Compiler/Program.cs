using Compiler.Diagnostics;
using Compiler.Syntax;

var src = @"
// This is a comment
use System.Console

fun main() {
    var x = '\n'
    var y = 1234 // Integer literal
    var f = 1.0f
    writeLine(""Hello, World!"")
    val s = $""Hello, {y}!""
    val e = ""This is an escaped \u0065 and an escaped \u6321""
}
";

var sourceReader = new SourceReader(src);
var lexer = new Lexer(sourceReader);
var tokens = new List<SyntaxToken>();

while (true)
{
    var token = lexer.Lex();
    tokens.Add(token);
    if (token.Kind == SyntaxKind.EndOfFile)
        break;
}

foreach (var token in tokens)
{
    Console.WriteLine(token);
}

var diagnostics = tokens
    .SelectMany(token => token.Diagnostics ?? Enumerable.Empty<DiagnosticInfo>())
    .ToList();

if (diagnostics.Any())
{
    Console.WriteLine();
    Console.WriteLine("Diagnostics:");
    foreach (var diagnostic in diagnostics)
    {
        Console.WriteLine(diagnostic);
    }
}
