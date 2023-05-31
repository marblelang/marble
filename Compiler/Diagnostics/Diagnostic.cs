namespace Compiler.Diagnostics;

internal record Diagnostic(string Code, string Title, string Format, DiagnosticCategory Category, DiagnosticSeverity Severity)
{
    public static string CreateDiagnosticCode(DiagnosticCategory category, int index) => $"MB{(int)category}{index:D3}";
}
