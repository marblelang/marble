using Compiler.Syntax;

var src = @"
// This is a comment
use System.Console

fun main() {
    var x = '\n'
    WriteLine(""Hello, World!"")
}
";

var sourceReader = new SourceReader(src);
var lexer = new Lexer(sourceReader);

while (true)
{
    var token = lexer.Lex();
    if (token.Kind == SyntaxKind.EndOfFile)
        break;
    Console.WriteLine(token);
}

if (lexer.Diagnostics.Count > 0)
{
    Console.WriteLine();
    Console.WriteLine("Diagnostics:");
    foreach (var diagnostic in lexer.Diagnostics)
        Console.WriteLine(diagnostic);
}
