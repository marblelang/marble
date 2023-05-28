namespace Compiler;

internal enum DiagnosticSeverity
{
    Error,
    Warning,
    Info,
    Hint
}

internal sealed record Diagnostic(DiagnosticSeverity Severity, string Message, int Line, int Column)
{
    public override string ToString() => $"{Severity}: {Message} ({Line}, {Column})";
}
