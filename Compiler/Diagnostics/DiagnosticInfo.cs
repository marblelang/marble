namespace Compiler.Diagnostics;

internal sealed record DiagnosticInfo(Diagnostic Diagnostic, int Offset, int Width, params object?[] Arguments)
{
    public override string ToString() => $"{Diagnostic.Severity} {Diagnostic.Code}: {Diagnostic.Title} at {Offset} with width {Width}";
}
