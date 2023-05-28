namespace Compiler.Syntax;

internal sealed class SourceReader
{
    public int Position { get; private set; }

    private readonly ReadOnlyMemory<char> _source;

    public SourceReader(string source)
    {
        _source = source.AsMemory();
    }

    /// <summary>
    /// Peeks at the next character in the source.
    /// </summary>
    /// <param name="offset">The offset from the current position.</param>
    /// <returns>The next character in the source.</returns>
    public char Peek(int offset = 0) => Position + offset >= _source.Length ? '\0' : _source.Span[Position + offset];

    /// <summary>
    /// Reads the next character(s) from the source.
    /// </summary>
    /// <param name="length">The number of characters to read.</param>
    /// <returns>The next character(s) from the source.</returns>
    public ReadOnlyMemory<char> Read(int length)
    {
        var result = _source.Slice(Position, length);
        Position += length;
        return result;
    }
}
